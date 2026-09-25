using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Department;
using RepositoryLayer.IAM.IDepartment;

namespace DataLayer.IAM.Department
{
    internal class DepartmentService:IDepartmentService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DepartmentService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(DepartmentRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_department_name", objmodel.Department_Name);
            dynParameters.Add("p_department_code", objmodel.Department_Code);
            dynParameters.Add("p_department_desc", objmodel.Department_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_department_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(DepartmentRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_department_id", objmodel.Department_Id);
            dynParameters.Add("p_department_name", objmodel.Department_Name);
            dynParameters.Add("p_department_code", objmodel.Department_Code);
            dynParameters.Add("p_department_desc", objmodel.Department_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_department_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<DepartmentResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<DepartmentResponseModel>("iam.sp_department_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<DepartmentResponseModel>> GetById(int Department_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_department_id", Department_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<DepartmentResponseModel>("iam.sp_department_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
