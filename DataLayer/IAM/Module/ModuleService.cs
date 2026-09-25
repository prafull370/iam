using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Module;
using RepositoryLayer.IAM.IModule;

namespace DataLayer.IAM.Module
{
    internal class ModuleService:IModuleService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ModuleService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ModuleRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_module_name", objmodel.Module_Name);
            dynParameters.Add("p_module_logo", objmodel.Module_Logo);
            dynParameters.Add("p_module_desc", objmodel.Module_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_module_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(ModuleRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_module_id", objmodel.Module_Id);
            dynParameters.Add("p_module_name", objmodel.Module_Name);
            dynParameters.Add("p_module_logo", objmodel.Module_Logo);
            dynParameters.Add("p_module_desc", objmodel.Module_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_module_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<ModuleResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<ModuleResponseModel>("iam.sp_module_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<ModuleResponseModel>> GetById(int Module_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_module_id", Module_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<ModuleResponseModel>("iam.sp_module_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
