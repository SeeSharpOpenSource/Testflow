using System.IO;
using System.Text.RegularExpressions;
using Testflow.Data;

namespace Testflow.SlaveCore.Common
{
    internal static class ModuleUtils
    {
        public static bool IsAbosolutePath(string path)
        {
            char dirDelim = Path.DirectorySeparatorChar;
            // 绝对路径匹配模式，如果匹配则path已经是绝对路径
            string regexFormat = dirDelim.Equals('\\')
                ? $"^(([a-zA-z]:)?{dirDelim}{dirDelim})"
                : $"^(([a-zA-z]:)?{dirDelim})";
            Regex regex = new Regex(regexFormat);
            return regex.IsMatch(path);
        }

        public static string GetFileFullPath(string path, string parentPath)
        {
            //            if (IsAbosolutePath(path))
            //            {
            //                return File.Exists(path) ? path : null;
            //            }
            if (string.IsNullOrWhiteSpace(parentPath))
            {
                return File.Exists(path) ? path : null;
            }
            string dirDelim = Path.DirectorySeparatorChar.ToString();
            if (!parentPath.EndsWith(dirDelim))
            {
                parentPath += dirDelim;
            }
            if (path.StartsWith(dirDelim))
            {
                path = path.Remove(path.Length - 1, 1);
            }
            string fullPath = parentPath + path;
            return File.Exists(fullPath) ? fullPath : null;
        }

        public static string GetTypeFullName(ITypeData typeData)
        {
            return $"{typeData.Namespace}.{typeData.Name}";
        }
    }
}