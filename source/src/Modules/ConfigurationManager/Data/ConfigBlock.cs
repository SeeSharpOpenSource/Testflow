using System.Collections.Generic;

namespace Testflow.ConfigurationManager.Data
{
    public class ConfigBlock : List<ConfigItem>
    {
        public string Name { get; set; }
    }
}