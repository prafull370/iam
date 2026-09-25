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
using ModelLayer.Models.IAM.RolePageMapping;
using RepositoryLayer.IAM.IRolePageMapping;

namespace DataLayer.IAM.RolePageMapping
{
    internal class RolePageMappingService:IRolePageMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public RolePageMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(RolePageMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
                           objmodel.RolePage_Data.Select(x => new
                           {
                               product_id = x.Product_Id,
                               service_id = x.Service_Id,
                               module_id = x.Module_Id,
                               page_id = x.Page_Id,
                               is_landing_page = x.Is_Landing_Page
                           }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_role_id", objmodel.Role_Id);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_role_product_service_module_page_mapping_save", dynParameters, LoggedUserData);
        }


        public async Task<ResponseModel<List<RolePageMappingResponseModel>>> Get(int Role_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_role_id",Role_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<RolePageMappingResponseModel>("iam.sp_role_product_service_module_page_mapping_get", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<List<AppRolePageMappingResponseModel>>> AppGet(int Role_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_role_id", Role_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<AppRolePageMappingResponseModel>("iam.sp_role_app_service_page_mapping_get", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> AppSave(AppRolePageMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
                           objmodel.RolePage_Data.Select(x => new
                           {
                               app_id = x.App_Id,
                               service_id = x.Service_Id,
                               page_id = x.Page_Id,
                               is_landing_page = x.Is_Landing_Page
                           }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_role_id", objmodel.Role_Id);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_role_app_service_page_mapping_save", dynParameters, LoggedUserData);
        }
    }
}
