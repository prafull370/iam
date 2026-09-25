using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Company;
using RepositoryLayer.IAM.ICompany;

namespace DataLayer.IAM.Company
{
    internal class CompanyService:ICompanyService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public CompanyService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(CompanyRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            // Adding all the parameters for the stored procedure
            
            dynParameters.Add("p_plan_id", objmodel.Plan_Id);
            dynParameters.Add("p_company_name", objmodel.Company_Name);
            dynParameters.Add("p_company_short_name", objmodel.Company_Short_Name);
            dynParameters.Add("p_company_name_regional", objmodel.Company_Name_Regional);
            dynParameters.Add("p_toll_free_number", objmodel.Toll_Free_Number);
            dynParameters.Add("p_company_website", objmodel.Company_Website);
            dynParameters.Add("p_address", objmodel.Address);
            dynParameters.Add("p_contact_mobile_no", objmodel.Contact_Mobile_No);
            dynParameters.Add("p_contact_email_id", objmodel.Contact_Email_Id);
            dynParameters.Add("p_contact_person_name", objmodel.Contact_Person_Name);
            dynParameters.Add("p_company_logo", objmodel.Company_Logo);
            dynParameters.Add("p_gst", objmodel.Gst);
            dynParameters.Add("p_tin", objmodel.Tin);
            dynParameters.Add("p_cin", objmodel.Cin);
            dynParameters.Add("p_company_desc", objmodel.Company_Desc);
            dynParameters.Add("p_remarks", objmodel.Remarks);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_ip_address", objmodel.Ip_Address);
            dynParameters.Add("p_password", objmodel.Password);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_company_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(CompanyRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_company_id", objmodel.Company_Id);
            dynParameters.Add("p_plan_id", objmodel.Plan_Id);
            dynParameters.Add("p_company_name", objmodel.Company_Name);
            dynParameters.Add("p_company_short_name", objmodel.Company_Short_Name);
            dynParameters.Add("p_company_name_regional", objmodel.Company_Name_Regional);
            dynParameters.Add("p_toll_free_number", objmodel.Toll_Free_Number);
            dynParameters.Add("p_company_website", objmodel.Company_Website);
            dynParameters.Add("p_address", objmodel.Address);
            dynParameters.Add("p_contact_mobile_no", objmodel.Contact_Mobile_No);
            dynParameters.Add("p_contact_email_id", objmodel.Contact_Email_Id);
            dynParameters.Add("p_contact_person_name", objmodel.Contact_Person_Name);
            dynParameters.Add("p_company_logo", objmodel.Company_Logo);
            dynParameters.Add("p_gst", objmodel.Gst);
            dynParameters.Add("p_tin", objmodel.Tin);
            dynParameters.Add("p_cin", objmodel.Cin);
            dynParameters.Add("p_company_desc", objmodel.Company_Desc);
            dynParameters.Add("p_remarks", objmodel.Remarks);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_ip_address", objmodel.Ip_Address);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_company_update", dynParameters, LoggedUserData);

        }
       
        public async Task<ResponseModel<List<CompanyResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<CompanyResponseModel>("iam.sp_company_get", dynParameters, pagination, LoggedUserData);
        }
       
        public async Task<ResponseModel<CompanyResponseModel>> GetById(int Company_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_company_id", Company_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<CompanyResponseModel>("iam.sp_company_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
