using Testflow.Modules;
using Testflow.Usr;

namespace Testflow.ConfigurationManager
{
    public class PlatformConfigManager : IConfigurationManager
    {
        public IModuleConfigData ConfigData { get; set; }
        public void RuntimeInitialize()
        {
            throw new System.NotImplementedException();
        }

        public void DesigntimeInitialize()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            throw new System.NotImplementedException();
        }

        public IPropertyExtendable GlobalInfo { get; set; }
        public void LoadConfigurationData()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyConfigData(IController controller)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}