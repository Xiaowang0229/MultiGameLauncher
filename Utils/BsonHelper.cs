using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RocketLauncherRemake.Utils
{
    public static class BsonHelper
    {

        public static void WriteBson<T>(T obj,string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // 确保目录存在
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using var fileStream = File.Create(filePath);           // 覆盖写入
            using var bsonWriter = new BsonDataWriter(fileStream);

            var serializer = JsonSerializer.CreateDefault();       // 或自定义设置
            serializer.Serialize(bsonWriter, obj);

            // 文件会在 using 结束后自动关闭 + 刷盘
        }
        public static T ReadBson<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("BSON 文件不存在", filePath);

            using var fileStream = File.OpenRead(filePath);
            using var bsonReader = new BsonDataReader(fileStream);

            var serializer = JsonSerializer.CreateDefault();       // 或自定义设置
            return serializer.Deserialize<T>(bsonReader)
                ?? throw new InvalidOperationException("反序列化结果为 null");
        }
    }
}
