using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Testflow.Usr;
using Testflow.Runtime.Data;

namespace Testflow.ResultManager.Common
{
    static class ModuleUtil
    {
        /// <summary>
        /// 确认路径是文件还是目录
        /// 如果是目录则创建Results.txt文件，返回
        /// 如果是文件，返回
        /// 路径不正常则抛出异常 
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回文件路径</returns>        
        internal static string CheckFilePath(string filePath)
        {
            if (!ModuleUtil.IsTxtFile(filePath))
            {
                if (!ModuleUtil.IsValidDirectory(filePath))
                {
                    throw new TestflowDataException(ModuleErrorCode.InvalidFilePath, $"Invalid File or Directory Path: {filePath}");
                }
                filePath += "Results.txt";
            }
            return filePath;
        }

        //todo需改变txt file
        private static bool IsTxtFile(string path)
        {
            return path.Contains(".txt") || File.Exists(path);
        }

        private static bool IsValidDirectory(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }
          
            int pathIndex = filePath.LastIndexOf(Path.DirectorySeparatorChar);
            return Directory.Exists(filePath.Substring(0, pathIndex));
        }

        /// <summary>
        /// 返回一个session里面的sequence的最大、最小ProcessorTime
        /// </summary>
        /// <param name="performanceList"></param>
        /// <returns> 大小为2的数组，{0}为最大ProcessorTime，{1}为最小ProcessorTime </returns>
        internal static double[] getMaxMinProcessorTime(IList<PerformanceStatus> performanceList)
        {
            double max = performanceList[0].ProcessorTime;
            double min = performanceList[0].ProcessorTime;
            foreach(PerformanceStatus status in performanceList)
            {
                if(status.ProcessorTime > max)
                {
                    max = status.ProcessorTime;
                }
                if (status.ProcessorTime < min)
                {
                    min = status.ProcessorTime;
                }
            }
            return new double[2] {max, min};
        }

        /// <summary>
        /// 返回一个session里面的sequence的最大、最小、平均MemoryUsed
        /// </summary>
        /// <param name="performanceList"></param>
        /// <returns> 大小为3的数组，{0}为最大MemoryUsed，{1}为最小MemoryUsed，{2}为平均MemoryUsed </returns>
        internal static long[] getMaxMinAveMemoryUsed(IList<PerformanceStatus> performanceList)
        {
            long max = performanceList[0].MemoryUsed;
            long min = performanceList[0].MemoryUsed;
            long ave = 0;
            foreach (PerformanceStatus status in performanceList)
            {
                if (status.ProcessorTime > max)
                {
                    max = status.MemoryUsed;
                }
                if (status.ProcessorTime < min)
                {
                    min = status.MemoryUsed;
                }
                ave += status.MemoryUsed;
            }
            return new long[3] { max, min, (ave / performanceList.Count) };
        }
    }
}
