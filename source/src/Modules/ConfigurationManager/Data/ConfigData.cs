using System.Collections.Generic;
using System.Xml.Serialization;

namespace Testflow.ConfigurationManager.Data
{
    [XmlType("ConfigurationData")]
    public class ConfigData
    {
        public ConfigData()
        {
            this.ModuleConfigData = new List<ConfigBlock>(10);
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ConfigVersion { get; set; }

        [XmlElement]
        public List<ConfigBlock> ModuleConfigData { get; set; }
    }
}