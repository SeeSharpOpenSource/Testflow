using System.Xml.Serialization;

namespace Testflow.ConfigurationManager.Data
{
    [XmlType("ConfigurationPair")]
    public class ConfigItem
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

        [XmlAttribute]
        public string Type { get; set; }
    }
}