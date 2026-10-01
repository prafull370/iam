using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.AppAuditLog;
using RepositoryLayer.IAM.IAppAuditLog;

namespace DataLayer.IAM.AppAuditLog
{
    internal class AppAuditLogService : IAppAuditLogService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;

        public AppAuditLogService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }

        // Unique app users (paginated). p_user_id, p_search_val, p_flag, p_start, p_length,
        // p_sort_column, p_sort_by, p_total_records are added by DBManager.

        #region Unique app users (grid)
        public async Task<ResponseModel<List<AppAuditUserResponseModel>>> GetUsers(AppAuditLogRequestModel model)
        {
            model.Flag = "LIST";
            model.Search_Val ??= "";

            DynamicParameters param = new DynamicParameters();
            param.Add("p_from_date", model.From_Date ?? "");
            param.Add("p_to_date", model.To_Date ?? "");

            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<AppAuditUserResponseModel>(
                "iam.sp_app_audit_log_get", param, model, LoggedUserData);
        }
        #endregion

        #region All actions of one user (drawer)
        public async Task<ResponseModel<List<AppAuditDetailResponseModel>>> GetDetail(string loginId)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("p_login_id", loginId);

            return await FactoryService.dbmanager.Execute_SP_Select_List<AppAuditDetailResponseModel>(
                "iam.sp_app_audit_log_detail_get", param, LoggedUserData);
        }
        #endregion


        #region Summary cards
        public async Task<ResponseModel<AppAuditStatsResponseModel>> GetStats()
        {
            DynamicParameters param = new DynamicParameters();

            return await FactoryService.dbmanager.Execute_SP_Select_Single<AppAuditStatsResponseModel>(
                "iam.sp_app_audit_log_stats_get", param, LoggedUserData);
        }
        #endregion

    }
}
