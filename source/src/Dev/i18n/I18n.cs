using System.Collections.Concurrent;
using System.Collections.Generic;
using Testflow.DataInterface;
using Testflow.Runtime;

namespace Testflow.i18n
{
    public class I18n
    {
        private static ConcurrentDictionary<long, I18n> _i18nEntities = new ConcurrentDictionary<long, I18n>();
        private IRuntimeContext context;

        public I18n(IRuntimeContext context)
        {
            this.context = context;
            // TODO to be implemented
        }

        public static I18n GetInstance(IRuntimeContext context)
        {
            long sessionId = context.SessionId;
            if (!_i18nEntities.ContainsKey(sessionId))
            {
                _i18nEntities.TryAdd(sessionId, new I18n(context));
            }
            return _i18nEntities[context.SessionId];
        }
    }
}
