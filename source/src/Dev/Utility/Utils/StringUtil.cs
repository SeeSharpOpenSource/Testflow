using System.IO;

namespace Testflow.Utility.Utils
{
    /// <summary>
    /// 字符串相关处理的工具类
    /// </summary>
    public static class StringUtil
    {
        #region Static Fields

        //  非法的变量名和表达式名
        //        private static char[] _invalidVariableNameChars;
        // 路径中的冗余符号
        private static readonly string RedundantPathDelim;

        private static readonly string PathDelim;

        static StringUtil()
        {
            RedundantPathDelim = $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}";
            PathDelim = Path.DirectorySeparatorChar.ToString();

            //            _invalidVariableNameChars = new char[]
            //            {
            //                '"', '\'', ',', '.',';', ':', '!',  '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+', '=', '[',
            //                ']', '{', '}', '|', '>', '<', '?', '/', '\\'
            //            };
        }

        #endregion

        /// <summary>
        /// 规范化文件路径
        /// </summary>
        public static string NormalizeFilePath(string filePath)
        {
            // 如果路径中包含冗余的分隔符，将分隔符替换
            while (filePath.Contains(RedundantPathDelim))
            {
                filePath = filePath.Replace(RedundantPathDelim, PathDelim);
            }
            return filePath;
        }

        /// <summary>
        /// 规范化文件夹
        /// </summary>
        public static string NomalizeDirectory(string directory)
        {
            if (!directory.EndsWith(PathDelim))
            {
                directory += PathDelim;
            }
            return directory;
        }
    }
}