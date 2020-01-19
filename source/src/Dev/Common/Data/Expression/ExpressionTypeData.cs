using System;
using System.Xml.Serialization;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式类型信息
    /// </summary>
    [Serializable]
    public class ExpressionTypeData
    {
        [XmlElement]
        public string AssemblyPath { get; set; }

        [XmlAttribute]
        public string ClassName { get; set; }
    }
}