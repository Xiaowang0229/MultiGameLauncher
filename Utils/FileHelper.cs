using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static string VersionLog;
        public static string EULAString;
        public static string BackgroundPath = $"{Environment.CurrentDirectory}\\Backgrounds";
    }
    public static class FileHelper
    {
        public static string ReadEmbeddedMarkdown(string resourceName)
        {
            // 获取当前执行的程序集
            var assembly = Assembly.GetExecutingAssembly();

            // 获取嵌入资源的流
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Resource {resourceName} not found.");

                // 使用 StreamReader 读取流中的文本
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public async static Task CopyFileAsync(string source, string dest)
        {
            if (Variables._MainWindow != null)
            {

                Variables._MainWindow.Tip.IsVisible = true;

            }

            await Task.Run(() => File.Copy(source, dest, overwrite: true));
            if (Variables._MainWindow != null)
            {
                Variables._MainWindow.Tip.IsVisible = false;

            }

        }

        public static async Task CopyDirectoryAsync(
        string sourceDir,
        string destinationDir,
        bool overwrite = true,
        int maxConcurrency = 8)
        {
            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException($"源目录不存在: {sourceDir}");
            }

            // 确保目标目录存在
            Directory.CreateDirectory(destinationDir);

            var source = new DirectoryInfo(sourceDir);
            var dest = new DirectoryInfo(destinationDir);

            // 1. 拷贝当前目录所有文件（并发）
            FileInfo[] files = source.GetFiles();

            var semaphore = new SemaphoreSlim(maxConcurrency);
            var tasks = new List<Task>(files.Length);

            foreach (FileInfo file in files)
            {
                string destFilePath = Path.Combine(dest.FullName, file.Name);

                tasks.Add(CopyFileWithSemaphoreAsync(
                    file.FullName,
                    destFilePath,
                    overwrite,
                    semaphore));
            }

            await Task.WhenAll(tasks);

            // 2. 递归处理子目录（串行递归，但每个目录内部文件并发）
            DirectoryInfo[] subDirs = source.GetDirectories();

            foreach (DirectoryInfo subDir in subDirs)
            {
                string newDestDir = Path.Combine(dest.FullName, subDir.Name);

                // 递归调用（注意：这里是串行递归，不是并行所有子目录）
                await CopyDirectoryAsync(
                    subDir.FullName,
                    newDestDir,
                    overwrite,
                    maxConcurrency);
            }
        }

        private static async Task CopyFileWithSemaphoreAsync(
            string sourceFile,
            string destFile,
            bool overwrite,
            SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                // 如果目标文件存在且只读，先解除只读（可选）
                if (File.Exists(destFile) && (File.GetAttributes(destFile) & FileAttributes.ReadOnly) != 0)
                {
                    File.SetAttributes(destFile, FileAttributes.Normal);
                }

                // 使用 FileStream 实现真正的异步拷贝（File.Copy 本身不是异步的）
                await using var sourceStream = new FileStream(
                    sourceFile,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 81920,      // 80KB 缓冲，比较合适
                    useAsync: true);

                await using var destStream = new FileStream(
                    destFile,
                    FileMode.Create,        // 或根据 overwrite 决定 Create / Open
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

                await sourceStream.CopyToAsync(destStream, 81920);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
