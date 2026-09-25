using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Action;
using RepositoryLayer.IAM.IAction;

namespace DataLayer.IAM.Action
{
    internal class ActionService:IActionService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ActionService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ActionRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_action_name", objmodel.Action_Name);
            dynParameters.Add("p_action_desc", objmodel.Action_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_action_code", objmodel.Action_Code);

            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_action_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(ActionRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_action_id", objmodel.Action_Id);
            dynParameters.Add("p_action_name", objmodel.Action_Name);
            dynParameters.Add("p_action_desc", objmodel.Action_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_action_code", objmodel.Action_Code);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_action_update", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<List<ActionResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<ActionResponseModel>("iam.sp_action_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<ActionResponseModel>> GetById(int Action_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_action_id", Action_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<ActionResponseModel>("iam.sp_action_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
