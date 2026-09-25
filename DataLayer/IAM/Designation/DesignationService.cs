using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Designation;
using RepositoryLayer.IAM.IDesignation;

namespace DataLayer.IAM.Designation
{
    internal class DesignationService:IDesignationService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DesignationService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(DesignationRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_department_id", objmodel.Department_Id);
            dynParameters.Add("p_designation_name", objmodel.Designation_Name);
            dynParameters.Add("p_designation_code", objmodel.Designation_Code);
            dynParameters.Add("p_designation_desc", objmodel.Designation_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_designation_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(DesignationRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_designation_id", objmodel.Designation_Id);
            dynParameters.Add("p_department_id", objmodel.Department_Id);
            dynParameters.Add("p_designation_name", objmodel.Designation_Name);
            dynParameters.Add("p_designation_code", objmodel.Designation_Code);
            dynParameters.Add("p_designation_desc", objmodel.Designation_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_designation_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<DesignationResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<DesignationResponseModel>("iam.sp_designation_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<DesignationResponseModel>> GetById(int Designation_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_designation_id", Designation_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<DesignationResponseModel>("iam.sp_designation_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
