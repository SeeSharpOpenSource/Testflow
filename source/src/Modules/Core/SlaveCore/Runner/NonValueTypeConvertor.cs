using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Testflow.CoreCommon;
using Testflow.SlaveCore.Common;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner
{
    internal class NonValueTypeConvertor
    {
        private readonly SlaveContext _context;
        private JsonSerializerSettings _settings;

        private const string ArrayDataRegex = @"^\[((^\[\],)*,)*(^\[\],)?\]$";
        private const string ClassDataRegex = "^\\{(\"(^\"\\{\\}:)+\":\"(^\":\\{\\})\",)+(\"(^\"\\{\\}:)+\":\"(^\":\\{\\})\")?}$";

        private Regex _arrayRegex;
        private Regex _classRegex;

        public NonValueTypeConvertor(SlaveContext context)
        {
            this._context = context;
            _settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = CommonConst.GlobalTimeFormat,
                DateParseHandling = DateParseHandling.None
            };
            _arrayRegex = new Regex(ArrayDataRegex);
            _classRegex = new Regex(ClassDataRegex);
        }

        public object CastConstantValue(Type targetType, string objStr)
        {
            if (targetType.IsInterface || targetType.IsAbstract)
            {
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                    _context.I18N.GetFStr("CastInterface", targetType.Name));
            }
            object castedObject = null;
            try
            {
                castedObject = JsonConvert.DeserializeObject(objStr, targetType, _settings);
            }
            catch (ApplicationException ex)
            {
                throw new TestflowDataException(ModuleErrorCode.UnsupportedTypeCast,
                    _context.I18N.GetFStr("CastInterface", targetType.Name), ex);
            }
            return castedObject;
        }

        public bool IsNonValueTypeString(ref string valueString)
        {
            return _arrayRegex.IsMatch(valueString) || _classRegex.IsMatch(valueString);
        }
    }
}