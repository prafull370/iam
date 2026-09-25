using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Vendor
{
    public class VendorRequestModel
    {
        public int Vendor_Id { get; set; }
        public string? Vendor_Name { get; set; }
        public int Country_Id { get; set; }
        public int State_Id { get; set; }
        public int District_Id { get; set; }
        public string? City_Name { get; set; }
        public string? Pin_Code { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Vendor_Desc { get; set; }
        public string? Contact_Person_Name { get; set; }
        public string? Contact_Person_Email_Id { get; set; }
        public string? Contact_Person_Mobile_No { get; set; }
        public bool Is_Active { get; set; } = true;
    }
    public class VendorResponseModel
    {
        public int Vendor_Id { get; set; }
        public string? ENC_Vendor_Id { get; set; }
        public string? Vendor_Name { get; set; }
        public int Country_Id { get; set; }
        public string? Country_Name { get; set; }
        public int State_Id { get; set; }
        public string? State_Name { get; set; }
        public int District_Id { get; set; }
        public string? District_Name { get; set; }
        public string? City_Name { get; set; }
        public string? Pin_Code { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Vendor_Desc { get; set; }
        public string? Contact_Person_Name { get; set; }
        public string? Contact_Person_Email_Id { get; set; }
        public string? Contact_Person_Mobile_No { get; set; }
        public bool Is_Active { get; set; }
        public bool Vendor_sync { get; set; }
        public string? Created_On { get; set; }
    }
}
