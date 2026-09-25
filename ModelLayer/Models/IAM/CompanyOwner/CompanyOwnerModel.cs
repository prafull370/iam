using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModelLayer.Models.Common;

namespace ModelLayer.Models.IAM.CompanyOwner
{
    public class CompanyOwnerViewModel
    {
        public CompanyOwnerRequestModel User_Model { get; set; }
        public List<CompanyOwnerHierarchy> CompanyOwner_Hierarchy { get; set; }
        public List<AppPermissionModel> AppPermissionModel { get; set; }
    }
    public class CompanyOwnerRequestModel
    {
        public int User_Id { get; set; } = 0;
        public string? User_Name { get; set; }
        public string? Mobile_No { get; set; }
        public string? Login_Id { get; set; }
        public string? Email_Id { get; set; }
        public string? Join_Date { get; set; }
        public int Department_Id { get; set; }
        public int Designation_Id { get; set; }
        public int? Authentication_Type_Id { get; set; }
        public int? Reporting_User_Id { get; set; }
        public int? Vendor_Id { get; set; }
        public int? Block_Remark_Type_Id { get; set; }
        public int Identity_Type_Id { get; set; }
        public string? Identity_No { get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
        public string? User_Logo { get; set; }
        public List<CompanyOwnerHierarchy> CompanyOwner_Hierarchy { get; set; }
        public List<AppPermissionModel> CompanyOwner_Permission { get; set; }
        public string? Password { get; set; }
        public string? Flag { get; set; }
    }

    public class CompanyOwnerHierarchy
    {
        public int Discom_Id { get; set; }
        public int Level1_Id { get; set; }
        public int Level2_Id { get; set; }
        public int Level3_Id { get; set; }
        public int Level4_Id { get; set; }
        public int Level5_Id { get; set; }
        public int Level6_Id { get; set; }
    }

    public class AppPermissionModel
    {
        public int App_Id { get; set; }
        public bool? Otp_Status { get; set; }
        public bool? Is_Checked { get; set; }
        public string? Expiry_Date { get; set; }
    }

    public class CompanyOwnerPaginationModel : PaginationModel
    {
        public string? Status { get; set; }
    }

    public class CompanyOwnerResponseModel
    {
        public int User_Id { get; set; }
        public string? ENC_User_Id { get; set; }
        public string? User_Name { get; set; }
        public string? User_Code { get; set; }
        public string? Login_Id { get; set; }
        public string? Mobile_No { get; set; }
        public string? Department_Name { get; set; }
        public string? Designation_Name { get; set; }
        public string? Identity_type_name { get; set; }
        public string? Identity_No { get; set; }
        public string? Block_remark_type_name { get; set; }
        public string? Vendor_Name { get; set; }
        public string? Authentication_type_name { get; set; }
        public string? Reporting_User_Name { get; set; }
        public string? Created_On { get; set; }
        public bool? Status { get; set; }
        public string? User_Logo { get; set; }

    }

    public class CompanyOwnerApplicationModel
    {
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public int App_Id { get; set; }
        public string? Application_Id { get; set; }
        public string? Application_Code { get; set; }
        public string? App_Name { get; set; }
        public string? App_Version { get; set; }
        public bool Otp_Status { get; set; } = false;
        public bool Is_Checked { get; set; } = false;
        public string? App_Expiry { get; set; }

    }

    public class FiltersModel
    {
        public string? Status { get; set; }
        public string? Search { get; set; }
    }

    // --------------  Permission Models ---------------- //
    public class CompanyOwnerDiscomPermissionModel
    {
        public int Discom_Id { get; set; }
        public string? Discom_Name { get; set; }
        public string? Discom_Short_Code { get; set; }
        public string? Discom_Name_Regional { get; set; }
        public string? Discom_Logo { get; set; }
        public int e { get; set; }
    }

    public class CompanyOwnerProductPermissionModel
    {
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Product_Short_Name { get; set; }
        public string? Product_Logo { get; set; }
        public string? Product_Url { get; set; }
        public string? Product_Api_Url { get; set; }
        public int Display_Order { get; set; }
    }

    public class CompanyOwnerServicePermissionModel
    {
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Service_Logo { get; set; }
        public string? Service_Url { get; set; }
        public string? Service_Api_Url { get; set; }
        public int Display_Order { get; set; }
    }

    public class CompanyOwnerModulePermissionModel
    {
        public int Module_Id { get; set; }
        public string? Module_Name { get; set; }
        public string? Module_Logo { get; set; }
        public int Display_Order { get; set; }
    }

    public class CompanyOwnerPagePermissionModel
    {
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public string? Page_Url { get; set; }
        public bool? Is_Parent { get; set; }
        public int Parent_Id { get; set; }
        public string? Page_Logo { get; set; }
        public bool? Has_Child { get; set; }
        public bool? Is_Landing_Page { get; set; }
        public List<SubMenuModel>? subMenus { get; set; }

    }

    public class SubMenuModel
    {
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public int Parent_Id { get; set; }
        public string? Page_Url { get; set; }
    }

    // ------------------- Login Model-------------------- //
    public class CompanyOwnerLoginModel
    {
        public string? User_Name { get; set; }
        public string? Password { get; set; }
        public string? OTP { get; set; }
        public string? Token { get; set; }
        public int Login_Type { get; set; }
        public string? Expiry_Time { get; set; }
    }

    public class CompanyOwnerChangePasswordModel
    {
        public int User_Id { get; set; }
        public string? Login_Id { get; set; }
        public string? Current_Password { get; set; }
        public string? New_Password { get; set; }
        public string? Confirm_Password { get; set; }
        public string? Token { get; set; }
    }
    public class CompanyOwnerLoginTypeModel
    {
        public int User_Id { get; set; }
        public int Login_Type_Id { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
    }


    public class LoginResponseModel : CompanyOwnerResponseModel
    {
        public int Company_Id { get; set; }
        public string? Email_Id { get; set; }
        public string? Ip_Address { get; set; }
        public int Discom_Id { get; set; }
        public string? Company_Name { get; set; }
        public string? Password { get; set; }
        public bool Change_Password_Flag { get; set; }
        public bool Is_Superadmin { get; set; }
        public bool Is_Company_Owner { get; set; }
        public string? Attempt_Count { get; set; }
        public string? IAM_Token { get; set; }
        public string? API_Token { get; set; }
        public string? Expiry { get; set; }
        public string? Role { get; set; }
    }

    public class CompanyOwnerInfoModel
    {
        public int User_Id { get; set; }
        public string? User_Name { get; set; }
        public string? User_Code { get; set; }
        public string? User_Logo { get; set; }
        public int Company_Id { get; set; }
        public string? Company_Name { get; set; }
        public string? Company_Logo { get; set; }
        public int Discom_Id { get; set; }
        public string? Discom_Name { get; set; }
        public string? Discom_Short_Code { get; set; }
        public string? Discom_Logo { get; set; }
        public string? Email_Id { get; set; }
        public string? Mobile_No { get; set; }
        public string? Ip_Address { get; set; }
        public int[]? Role_Ids { get; set; }
        public string[]? Roles { get; set; }
    }

}
