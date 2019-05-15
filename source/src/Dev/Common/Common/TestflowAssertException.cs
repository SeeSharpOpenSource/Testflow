using System;
using System.Runtime.Remoting.Proxies;
using Testflow.Usr.I18nUtil;

namespace Testflow.Usr
{
    /// <summary>
    /// Testflow的断言异常
    /// </summary>
    [Serializable]
    public class TestflowAssertException : TestflowException
    {
        /// <summary>
        /// 创建TestflowAssertException的实例
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">异常信息</param>
        public TestflowAssertException(string message) : base(CommonErrorCode.AssertionFailed, message)
        {
        }

        /// <summary>
        /// 创建TestflowAssertException的实例
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="real"></param>
        public TestflowAssertException(string expected, string real): base(CommonErrorCode.AssertionFailed, 
            I18N.GetInstance(CommonConst.I18nName).GetFStr("AssertFailedInfo", expected, real))
        {
            
        }
    }
}