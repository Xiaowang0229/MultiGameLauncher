using System;
using System.Collections.Generic;
using System.Text;

namespace RocketLauncherRemake.Utils
{
    public static partial class Variables
    {
        public static string Version = "4.3.0-Fix";
        public static string VersionLog = "# Release 4.3.0 版本日志\r\n---\r\n- 修复了已知问题\r\n- 使用 Bson 存储配置文件,安全性提升\r\n";
        public static bool IsDevelopmentMode = false;
    }
}
