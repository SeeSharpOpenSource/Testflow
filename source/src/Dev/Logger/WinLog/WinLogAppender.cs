using System;
using System.Reflection;
using System.Runtime.Remoting;
using Testflow.Common;

namespace Testflow.Logger.WinLog
{
    /// <summary>
    /// 日志流
    /// </summary>
    internal class WinLogAppender : ObjectHandle, ILogAppender
    {
        public static WinLogAppender CreateWinLogStream(AppDomain appDomain)
        {
            Type logStreamType = typeof(WinLogAppender);
            Assembly currentAssembly = Assembly.GetAssembly(logStreamType);
            appDomain.Load(currentAssembly.FullName);
            return (WinLogAppender)appDomain.CreateInstance(currentAssembly.FullName, logStreamType.FullName);
        }


        public WinLogAppender(object o) : base(o)
        {
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            throw new NotImplementedException();
        }

        public void Print(LogLevel logLevel, int sequenceIndex, Exception exception)
        {
            throw new NotImplementedException();
        }

    }
}