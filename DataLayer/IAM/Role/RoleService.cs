using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Role;
using RepositoryLayer.IAM.IRole;

namespace DataLayer.IAM.Role
{
    internal class RoleService:IRoleService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public RoleService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(RoleRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_role_name", objmodel.Role_Name);
            dynParameters.Add("p_role_desc", objmodel.Role_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_role_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(RoleRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_role_id", objmodel.Role_Id);
            dynParameters.Add("p_role_name", objmodel.Role_Name);
            dynParameters.Add("p_role_desc", objmodel.Role_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_role_update", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<List<RoleResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<RoleResponseModel>("iam.sp_role_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<RoleResponseModel>> GetById(int Role_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_role_id", Role_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<RoleResponseModel>("iam.sp_role_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
