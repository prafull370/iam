using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using DocumentFormat.OpenXml.EMMA;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Department;
using ModelLayer.Models.IAM.ErrorLog;
using ModelLayer.Models.IAM.User;
using RepositoryLayer.IAM.IErrorLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.IAM.ErrorLog
{
    internal class ErrorLogService: IErrorLogService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ErrorLogService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }

        public async Task<ResponseModel<List<DBErrorLogResponseModel>>> GetList(DBErrorLogRequestModel model)
        {
            model.Flag = "LIST";

            DynamicParameters param = new DynamicParameters();
            // Search value from PaginationModel
            model.Search_Val ??= "";
            param.Add("p_from_date", model.FromDate ?? "");
            param.Add("p_to_date", model.EndDate ?? "");

            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<DBErrorLogResponseModel>("iam.sp_error_log_data_get",param,model,LoggedUserData);
        }
    }
}
