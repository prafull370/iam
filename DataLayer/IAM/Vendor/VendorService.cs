using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Vendor;
using RepositoryLayer.IAM.IVendor;

namespace DataLayer.IAM.Vendor
{
    internal class VendorService:IVendorService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public VendorService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(VendorRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_vendor_name", objmodel.Vendor_Name);
            dynParameters.Add("p_country_id", objmodel.Country_Id);
            dynParameters.Add("p_state_id", objmodel.State_Id);
            dynParameters.Add("p_district_id", objmodel.District_Id);
            dynParameters.Add("p_city_name", objmodel.City_Name);
            dynParameters.Add("p_address1", objmodel.Address1);
            dynParameters.Add("p_address2", objmodel.Address2);
            dynParameters.Add("p_contact_person_name", objmodel.Contact_Person_Name);
            dynParameters.Add("p_contact_person_email_id", objmodel.Contact_Person_Email_Id);
            dynParameters.Add("p_contact_person_mobile_no", objmodel.Contact_Person_Mobile_No);
            dynParameters.Add("p_pin_code", objmodel.Pin_Code);
            dynParameters.Add("p_vendor_desc", objmodel.Vendor_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_vendor_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(VendorRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_vendor_id", objmodel.Vendor_Id);
            dynParameters.Add("p_vendor_name", objmodel.Vendor_Name);
            dynParameters.Add("p_country_id", objmodel.Country_Id);
            dynParameters.Add("p_state_id", objmodel.State_Id);
            dynParameters.Add("p_district_id", objmodel.District_Id);
            dynParameters.Add("p_city_name", objmodel.City_Name);
            dynParameters.Add("p_address1", objmodel.Address1);
            dynParameters.Add("p_address2", objmodel.Address2);
            dynParameters.Add("p_contact_person_name", objmodel.Contact_Person_Name);
            dynParameters.Add("p_contact_person_email_id", objmodel.Contact_Person_Email_Id);
            dynParameters.Add("p_contact_person_mobile_no", objmodel.Contact_Person_Mobile_No);
            dynParameters.Add("p_pin_code", objmodel.Pin_Code);
            dynParameters.Add("p_vendor_desc", objmodel.Vendor_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_vendor_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<VendorResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<VendorResponseModel>("iam.sp_vendor_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<VendorResponseModel>> GetById(int Vendor_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_vendor_id", Vendor_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<VendorResponseModel>("iam.sp_vendor_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
