using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
namespace DataAccessLayer
{
    public static class DataAccessLayerDependency
    {
        public static void AddDataAccessLayerServices(this IServiceCollection services)
        {
            services.AddScoped<DataAccessDependencyService>(); // Change from Singleton to Scoped
            services.AddScoped<IDBConnection, DBConnection>();
            services.AddTransient<IDBFactoryService, DBFactoryService>();
            services.AddTransient<IDBManager, DBManager>();
        }
    }


    internal class DataAccessDependencyService
    {
        public IDBFactoryService dbfactory { get; }
        public IDBManager dbmanager { get; }

        public DataAccessDependencyService(
            IDBFactoryService DBFactory,
            IDBManager DBManager
        )
        {
            dbfactory = DBFactory;
            dbmanager = DBManager;
        }
    }
  
}
