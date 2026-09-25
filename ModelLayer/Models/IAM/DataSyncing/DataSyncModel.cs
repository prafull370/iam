using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;

namespace ModelLayer.Models.IAM.DataSyncing
{
    public class DataSyncRequestModel:PaginationModel
    {
        public int Sync_Type_Id { get; set; }
        public int Product_Id { get; set; }
        public bool? Status { get; set; }
    }
    public class ProductDataSyncModel
    {
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Product_Short_Name { get; set; }
        public string? Product_Url { get; set; }
        public string? Product_API_Url { get; set; }
    }
    public class DataSyncJob
    {
        public List<DataSyncResponseModel>? Records { get; set; }
        public int SyncTypeId { get; set; }
        public int ProductId { get; set; }
    }

    public class DataSyncResponseModel
    {
        public int Record_Id { get; set; }
        public string? Record_Name { get; set; }
        public bool? Sync_Status { get; set; }
        public string? Synced_On { get; set; }
        public string? Message { get; set; }
    }
    public class DataSyncLogModel
    {
        public int Record_Id { get; set; }
        public int Product_Id { get; set; }
        public bool? Sync_Status { get; set; }
        public int Sync_Type_Id { get; set; }
        public string? Message { get; set; }
    }
    public class UserSyncModel
    {
        public int? User_Id { get; set; }
        public int? Reporting_User_Id { get; set; }
        public string? Login_Id { get; set; }
        public string? User_Name { get; set; }
        public string? User_Code { get; set; }
        public string? Email_Id { get; set; }
        public string? User_Logo { get; set; }
        public string? Mobile_No { get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
        public string? Designation_Name { get; set; }
        public string? Designation_Code { get; set; }
        public string? Department_Name { get; set; }
        public string? Department_Code { get; set; }
        public string? Identity_Type { get; set; }
        public string? Identity_No { get; set; }
        public int? Created_By { get; set; }
        public int? Last_Updated_By { get; set; }
        public string? Ip_Address { get; set; }
        public int? Vendor_Id { get; set; }
        public int? Company_Id { get; set; }
        public string? Skill { get; set; }
        public string? Password { get; set; }
    }
    public class UserHierarchySyncModel
    {
        public int? User_Id { get; set; }
        public int? Created_By { get; set; }
        public int? Last_Updated_By { get; set; }
        public string? Ip_Address { get; set; }
        public List<HierarchySyncModel>? Hierarchy_List { get; set; }
    }
    public class HierarchySyncModel
    {
        
        public int? Company_Id { get; set; }
        public int? Discom_Id { get; set; }
        public int? Level1_Id { get; set; }
        public int? Level2_Id { get; set; }
        public int? Level3_Id { get; set; }
        public int? Level4_Id { get; set; }
        public int? Level5_Id { get; set; }
        public int? Level6_Id { get; set; }
    }
    public class HierarchyResponseModel
    {
        public int? User_Id { get; set; }
        public int? Created_By { get; set; }
        public int? Last_Updated_By { get; set; }
        public string? Ip_Address { get; set; }
        public int? Company_Id { get; set; }
        public int? Discom_Id { get; set; }
        public int? Level1_Id { get; set; }
        public int? Level2_Id { get; set; }
        public int? Level3_Id { get; set; }
        public int? Level4_Id { get; set; }
        public int? Level5_Id { get; set; }
        public int? Level6_Id { get; set; }
    }
    public class VendorSyncModel
    {
        public int Vendor_Id { get; set; }                 // vd.vendor_id
        public string? Vendor_Name { get; set; }            // vd.vendor_name
        public string? Country_Name { get; set; }           // ct.country_name
        public string? State_Name { get; set; }             // st.state_name
        public string? City_Name { get; set; }              // vd.city_name
        public string? Pin_Code { get; set; }               // vd.pin_code
        public string? Address1 { get; set; }               // vd.address1
        public string? Address2 { get; set; }               // vd.address2
        public string? Vendor_Desc { get; set; }            // vd.vendor_desc
        public string? Contact_Person_Email_Id { get; set; }   // vd.contact_person_email_id
        public string? Contact_Person_Mobile_No { get; set; }  // vd.contact_person_mobile_no
        public string? Contact_Person_Name { get; set; }       // vd.contact_person_name
        public string? Ip_Address { get; set; }             // vd.ip_address
        public bool Is_Active { get; set; }                // vd.is_active
        public int Created_By { get; set; }                // vd.created_by
        public int Last_Updated_By { get; set; }           // vd.last_updated_by
    }
    public class LevelSyncModel
    {
        public int Level_Id { get; set; }
        public int Prev_Level_Id { get; set; }
        public int Type_Id { get; set; }
        public string? Level_Name { get; set; }
        public string? Level_Code { get; set; }
        public bool Is_Active { get; set; }
        public int Created_By { get; set; }
        public int Last_Updated_By { get; set; }
        public string? Ip_Address { get; set; }
    }
}
