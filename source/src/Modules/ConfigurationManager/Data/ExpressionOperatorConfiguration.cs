using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Testflow.Data.Expression;

namespace Testflow.ConfigurationManager.Data
{
    [Serializable]
    public class ExpressionOperatorConfiguration
    {
        [XmlAttribute]
        public string Version { get; set; }

        [XmlArrayItem("ExpressionOperator")]
        [XmlArray("ExpressionOperators")]
        public List<ExpressionOperatorInfo> Operators { get; set; }

        [XmlArrayItem("ExpressionCalculatorInfo")]
        [XmlArray("ExpressionCalculators")]
        public List<ExpressionCalculatorInfo> Calculators { get; set; }
    }
}