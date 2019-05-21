using System.Data.Common;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testflow.DataMaintainerTest
{
    [TestClass]
    public class DataMaintainTest
    {
        private DbConnection _dbConnection;

        public DataMaintainTest()
        {
            const string dataBaseName = "testflowData.db3";
            if (File.Exists(dataBaseName))
            {
                File.Delete(dataBaseName);
            }
            
        }

        [TestInitialize]
        public void Initialize()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");
            _dbConnection = factory.CreateConnection();
            _dbConnection.ConnectionString = "Data Source=test.db3";
            _dbConnection.Open();
        }
    }
}