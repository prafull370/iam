using System.Data;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer
{
    internal class DBConnection : IDBConnection
    {
        private readonly IConfiguration _configuration;

        public DBConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("IAM_Con") ?? throw new InvalidOperationException("Connection string is not found.");
        }
    }
    internal interface IDBConnection
    {
        string GetConnectionString();
    }

}
