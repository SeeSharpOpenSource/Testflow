using System.IO;

namespace Testflow.Utility.Utils
{
    /// <summary>
    /// 路径相关处理的工具类
    /// </summary>
    public static class StringUtil
    {
        #region Static Fields

        //  非法的变量名和表达式名
        //        private static char[] _invalidVariableNameChars;
        // 路径中的冗余符号
        private static readonly string RedundantPathStr;

        private static readonly string PathDelim;

        static StringUtil()
        {
            RedundantPathStr = $"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}";
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
            while (filePath.Contains(RedundantPathStr))
            {
                filePath = filePath.Replace(RedundantPathStr, Path.DirectorySeparatorChar.ToString());
            }
            return filePath;
        }

        /// <summary>
        /// 规范化文件夹
        /// </summary>
        public static string NomalizeDirectory(string directory)
        {
            return directory.EndsWith(PathDelim) ? directory : directory + PathDelim;
        }
    }
}