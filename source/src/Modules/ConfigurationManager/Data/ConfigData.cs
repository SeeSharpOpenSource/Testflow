using System.Collections.Generic;

namespace Testflow.ConfigurationManager.Data
{
    public class ConfigData : List<ConfigBlock>
    {
        public string Name { get; set; }

        public string ConfigVersion { get; set; }
    }
}