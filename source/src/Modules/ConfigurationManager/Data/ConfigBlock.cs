using System.Collections.Generic;
using System.Xml.Serialization;

namespace Testflow.ConfigurationManager.Data
{
    [XmlType("ModuleConfiguration")]
    public class ConfigBlock
    {
        public ConfigBlock()
        {
            this.ConfigItems = new List<ConfigItem>(10);
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public List<ConfigItem> ConfigItems { get; set; }
    }
}