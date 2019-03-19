using System;
using Testflow.Common;
using Testflow.Utility.I18nUtil;

namespace Testflow.MasterCore.Common
{
    internal static class ModuleUtil
    {

        public static TDataType GetDeleage<TDataType>(Delegate action) where TDataType : class
        {
            TDataType delegateAction = action as TDataType;
            if (null == delegateAction)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowInternalException(ModuleErrorCode.IncorrectDelegate,
                    i18N.GetFStr("IncorrectDelegate", action.GetType().Name));
            }
            return delegateAction;
        }

        public static TDataType GetParamValue<TDataType>(object[] eventParams, int index) where TDataType : class
        {
            TDataType paramValueObject = null;
            if (eventParams.Length > index && null == (paramValueObject = eventParams[index] as TDataType))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowInternalException(ModuleErrorCode.IncorrectParamType,
                    i18N.GetFStr("IncorrectParamType", typeof(TDataType).Name));
            }

            return paramValueObject;
        }
    }
}