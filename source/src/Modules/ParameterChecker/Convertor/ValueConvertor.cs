using System;
using System.Collections.Generic;
using System.Linq;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;
using Testflow.Usr;

namespace Testflow.ParameterChecker
{
    internal static class ValueConvertor
    {
        private static Dictionary<string, Func<string, object>> _convertorHandler;
        //private static HashSet<ITypeData> _valueTypes;

        static ValueConvertor()
        {
            _convertorHandler = new Dictionary<string, Func<string, object>>(12)
            {
                {typeof (string).Name, (valueStr) => valueStr},
                {typeof (int).Name, (valueStr) => int.Parse(valueStr)},
                {typeof (uint).Name, (valueStr) => uint.Parse(valueStr)},
                {typeof (short).Name, (valueStr) => short.Parse(valueStr)},
                {typeof (ushort).Name, (valueStr) => ushort.Parse(valueStr)},
                {typeof (long).Name, (valueStr) => long.Parse(valueStr)},
                {typeof (ulong).Name, (valueStr) => ulong.Parse(valueStr)},
                {typeof (double).Name, (valueStr) => double.Parse(valueStr)},
                {typeof (byte).Name, (valueStr) => byte.Parse(valueStr)},
                {typeof (char).Name, (valueStr) => char.Parse(valueStr)},
                {typeof (bool).Name, (valueStr) => bool.Parse(valueStr)},
                {typeof (DateTime).Name, (valueStr) => DateTime.Parse(valueStr)}
            };

            #region 记录 
            //IComInterfaceManager _interfaceManager = TestflowRunner.GetInstance().ComInterfaceManager;
            //IComInterfaceDescription comDescription = _interfaceManager.GetComInterfaceByName("mscorlib");
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Boolean")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Double")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Single")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Int64")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("UInt64")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Int32")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Int16")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("UInt16")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Char")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("Byte")));
            //_valueTypes.Add(comDescription.VariableTypes.FirstOrDefault(item => item.Name.Equals("String")));
            #endregion
        }

        //internal static bool CheckForValueType(ITypeData typeData)
        //{
        //    if (_valueTypes.Contains(typeData))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}


        //todo I18n
        internal static bool CheckValue(string typeName, string Value)
        {
            try
            {
                _convertorHandler[typeName].Invoke(Value);
                return true;
            }
            catch(FormatException ex)
            {
                return false;
            }
            catch(Exception ex)
            {
                throw new TestflowDataException(ModuleErrorCode.InvalidType, "Type {typeName} must be of system value type");
            }
            
        }
    }
}