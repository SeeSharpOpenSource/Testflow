using System;
using Testflow.Data;

namespace Testflow.SequenceManager.SequenceElements
{
    public class AssemblyInfo : IAssemblyInfo
    {
        [Serializable]
        public AssemblyInfo()
        {
            this.AssemblyName = string.Empty;
            this.Path = string.Empty;
            this.Available = false;
            this.Version = string.Empty;
        }

        public string AssemblyName { get; set; }

        public string Path { get; set; }

        public bool Available { get; set; }

        public string Version { get; set; }

        public override bool Equals(object item)
        {
            IAssemblyInfo assemblyInfo = item as IAssemblyInfo;
            if (null == assemblyInfo)
            {
                return false;
            }
            return AssemblyName.Equals(assemblyInfo.AssemblyName) && Version.Equals(assemblyInfo.Version);
        }

        public override int GetHashCode()
        {
            return $"{AssemblyName}.{Version}".GetHashCode();
        }
    }
}