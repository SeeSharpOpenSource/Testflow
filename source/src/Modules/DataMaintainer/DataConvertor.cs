using System;
using System.Collections.Generic;

namespace Testflow.DataMaintainer
{
    internal static class DataConvertor
    {
        private static Dictionary<string, Func<object, object>> _convertors;
        static DataConvertor()
        {
            _convertors = new Dictionary<string, Func<object, object>>(10);
            // TODO
        }
    }
}