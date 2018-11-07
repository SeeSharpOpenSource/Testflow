using Testflow.Common;

namespace Testflow.Runtime
{
    public interface IRuntimeSession : IEntityComponent
    {
        /// <summary>
        /// 当前运行时的上下文信息
        /// </summary>
        IRuntimeContext Context { get; }

        void Start();

        void Stop();

        void Handle();
    }
}