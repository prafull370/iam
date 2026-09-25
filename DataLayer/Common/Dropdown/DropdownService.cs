using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Company;
using RepositoryLayer.Common.IDropdown;

namespace DataLayer.Common.Dropdown
{
    internal class DropdownService:IDropdownService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DropdownService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }

        public async Task<ResponseModel<List<DDLResponseModel>>> Get(DDLRequestModel model)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_id", model.Id);
            dynParameters.Add("p_flag", model.Flag);
            dynParameters.Add("p_ddl_flag", model.DDL_Flag);
            dynParameters.Add("p_exclude_id", model.Exclude_Id ?? 0);
            return await FactoryService.dbmanager.Execute_SP_Select_List<DDLResponseModel>("iam.sp_common_dropdown_get", dynParameters, LoggedUserData);

        }
    }
}
