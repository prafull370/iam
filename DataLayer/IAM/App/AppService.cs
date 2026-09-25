using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.App;
using ModelLayer.Models.IAM.Department;
using RepositoryLayer.IAM.IApp;

namespace DataLayer.IAM.App
{
    internal class AppService:IAppService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public AppService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(AppRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_application_id", objmodel.Application_Id);
            dynParameters.Add("p_application_code", objmodel.Application_Code);
            dynParameters.Add("p_app_name", objmodel.App_Name);
            dynParameters.Add("p_app_version", objmodel.App_Version);
            dynParameters.Add("p_app_min_sdk_version", objmodel.App_Min_Sdk_Version);
            dynParameters.Add("p_app_target_sdk_version", objmodel.App_Target_Sdk_Version);
            dynParameters.Add("p_app_play_store_url", objmodel.App_Play_Store_Url);
            dynParameters.Add("p_app_logo", objmodel.App_Logo);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_app_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(AppRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_app_id", objmodel.App_Id);
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_application_id", objmodel.Application_Id);
            dynParameters.Add("p_application_code", objmodel.Application_Code);
            dynParameters.Add("p_app_name", objmodel.App_Name);
            dynParameters.Add("p_app_version", objmodel.App_Version);
            dynParameters.Add("p_app_min_sdk_version", objmodel.App_Min_Sdk_Version);
            dynParameters.Add("p_app_target_sdk_version", objmodel.App_Target_Sdk_Version);
            dynParameters.Add("p_app_play_store_url", objmodel.App_Play_Store_Url);
            dynParameters.Add("p_app_logo", objmodel.App_Logo);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_app_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<AppResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<AppResponseModel>("iam.sp_app_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<AppResponseModel>> GetById(int App_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_app_id", App_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<AppResponseModel>("iam.sp_app_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
