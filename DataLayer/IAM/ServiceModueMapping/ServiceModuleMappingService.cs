using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ServiceModuleMapping;
using RepositoryLayer.IAM.IServiceModueMapping;

namespace DataLayer.IAM.ServiceModueMapping
{
    internal class ServiceModuleMappingService:IServiceModuleMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ServiceModuleMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ServiceModuleMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
            objmodel.Module_Ids.Select(pid => new { module_id = pid }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_service_id", objmodel.Service_Id);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_service_module_mapping_save", dynParameters, LoggedUserData);
        }


        public async Task<ResponseModel<List<ServiceModuleMappingResponseModel>>> Get(int Service_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_service_id", Service_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<ServiceModuleMappingResponseModel>("iam.sp_service_module_mapping_get", dynParameters, LoggedUserData);
        }
    }
}
