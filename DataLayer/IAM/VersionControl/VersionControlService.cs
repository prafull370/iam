using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.VersionControl;
using ModelLayer.Models.IAM.Department;
using RepositoryLayer.IAM.IVersionControl;

namespace DataLayer.IAM.VersionControl
{
    internal class VersionControlService:IVersionControlService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public VersionControlService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(VersionControlRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_version_point", objmodel.Version_Point);
            dynParameters.Add("p_app_id", objmodel.App_Id);
            dynParameters.Add("p_version", objmodel.Version);
            dynParameters.Add("p_created_on", objmodel.Created_On);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_page_desc", objmodel.Description);

            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_version_save", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<List<VersionControlResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<VersionControlResponseModel>("iam.sp_version_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<VersionControlResponseModel>> GetById(int Version_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_version_id", Version_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<VersionControlResponseModel>("iam.sp_version_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
