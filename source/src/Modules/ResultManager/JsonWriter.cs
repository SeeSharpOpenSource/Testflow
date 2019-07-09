using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Modules;

namespace Testflow.ResultManager
{
    internal class JsonWriter:IResultPrinter
    {
        /// <summary>
        /// 输出TestInstance的json report
        /// </summary>
        /// <param name="runtimeHash"></param>
        public void PrintReport(string filePath, IDataMaintainer dataMaintainer, string runtimeHash)
        {
            //todo
            throw new NotImplementedException("not implemented yet");
        }
    }
}
