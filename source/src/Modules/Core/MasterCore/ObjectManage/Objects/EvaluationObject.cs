using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.ObjectManage.Objects
{
    internal class EvaluationObject : RuntimeObject
    {
        public int Session { get; }

        public int Sequence { get; }

        public string Expression { get; }

        public EvaluationObject(int session, int sequence, string expression) : base(Constants.EvaluationObjectName)
        {
            this.Session = session;
            this.Sequence = sequence;
            this.Expression = expression;
        }
    }
}