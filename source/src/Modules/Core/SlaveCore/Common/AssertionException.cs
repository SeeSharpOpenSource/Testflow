using System;
using Testflow.CoreCommon.Data;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner.Model;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore.Common
{
    internal class AssertionException : ApplicationException
    {
        public CallStack Stack { get; }
        public string VariableName { get; set; }
        public string ExpectedValue { get; }
        public string RealValue { get; }

        public AssertionException(StepAssertModel stepModel) : base(
            I18N.GetInstance(Constants.I18nName).GetFStr("AssertFailMessage", stepModel.VariableName,
                stepModel.Expected, stepModel.RealValue))
        {
            this.Stack = stepModel.GetStack();
            this.VariableName = stepModel.VariableName;
            this.ExpectedValue = stepModel.Expected;
            this.RealValue = stepModel.RealValue;
        }
    }
}