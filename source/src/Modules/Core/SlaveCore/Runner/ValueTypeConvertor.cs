using System;
using Testflow.Data;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner
{
    internal class ValueTypeConvertor
    {
        private SlaveContext _context;

        public ValueTypeConvertor(SlaveContext context)
        {
            _context = context;
        }

        public object ConvertType(ITypeData sourceType, ITypeData targetType, object sourceValue)
        {
            throw new NotImplementedException();
        }
    }
}