using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Configuration;
using RepositoryLayer.IAM.IConfiguration;

namespace DataLayer.IAM.Configuration
{
    internal class ConfigurationService:IConfigurationService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ConfigurationService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ConfigurationRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_config_name", objmodel.Config_Name);
            dynParameters.Add("p_config_value", objmodel.Config_Value);
            dynParameters.Add("p_description", objmodel.Description);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_is_config_value", objmodel.Is_Config_Value);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("mst.sp_configuration_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(ConfigurationRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_config_id", objmodel.Config_Id);
            dynParameters.Add("p_config_name", objmodel.Config_Name);
            dynParameters.Add("p_config_value", objmodel.Config_Value);
            dynParameters.Add("p_description", objmodel.Description);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("mst.sp_configuration_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<ConfigurationResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<ConfigurationResponseModel>("mst.sp_configuration_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<ConfigurationResponseModel>> GetById(int Configuration_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_configuration_id", Configuration_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<ConfigurationResponseModel>("mst.sp_configuration_get_by_id", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<ConfigurationResponseModel>> GetByName(string Config_Name)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_config_name", Config_Name);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<ConfigurationResponseModel>("mst.sp_configuration_get_by_name", dynParameters, LoggedUserData);
        }
    }
}
