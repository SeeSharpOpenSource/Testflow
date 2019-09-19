using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;
using Testflow.Usr;

namespace Testflow.DesigntimeService.Common
{
    internal static class TypeChecker
    {
        private static HashSet<ITypeData> _valueTypes;

        static TypeChecker()
        {
            IComInterfaceManager _interfaceManager = TestflowRunner.GetInstance().ComInterfaceManager;
            IComInterfaceDescription comDescription = _interfaceManager.GetComInterfaceByName("mscorlib");
            _valueTypes = new HashSet<ITypeData>();
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Boolean")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Double")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Single")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Int64")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("UInt64")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Int32")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Int16")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("UInt16")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Char")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Byte")));
            _valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("String")));
        }

        internal static bool CheckForValueType(ITypeData typeData)
        {
            if (_valueTypes.Contains(typeData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
