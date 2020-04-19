using System;
using Testflow.Data;
using Testflow.Data.Sequence;

namespace Testflow.ResultManager
{
    internal class XMLWriter : IResultPrinter
    {
        /// <summary>
        /// 输出TestInstance的report到xml文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sequenceData"></param>
        /// <param name="runtimeHash"></param>
        public void PrintReport(string filePath, ISequenceFlowContainer sequenceData, string runtimeHash)
        {
            //todo
            throw new NotImplementedException("not implemented yet");
        }
    }
}
