using System;
using System.Xml.Serialization;

namespace Testflow.ConfigurationManager.Data
{
    [Serializable]
    public class ExpressionOperatorConfiguration
    {
        [XmlAttribute]
        public string Version { get; set; }

        [XmlArrayItem("ExpressionOperator")]
        [XmlArray("ExpressionOperators")]
        public ExpressionTokenCollection ExpressionOperators { get; set; }
    }
}