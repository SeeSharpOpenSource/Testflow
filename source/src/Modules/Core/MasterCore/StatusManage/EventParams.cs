namespace Testflow.MasterCore.StatusManage
{
    internal class EventParam
    {
        public string EventName { get; }

        public int Session { get; }

        public object[] EventParams { get; }

        public EventParam(string eventName, int session, object[] eventParams)
        {
            this.EventName = eventName;
            this.Session = session;
            this.EventParams = eventParams;
        }
    }
}