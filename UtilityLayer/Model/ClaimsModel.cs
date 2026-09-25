using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLayer.Model
{
    public class ClaimsModel
    {
        public string? User_Name { get; set; }
        public int User_Id { get; set; }
        public string? User_Code { get; set; }
        public int Company_Id { get; set; }
        public int Discom_Id { get; set; }
        public string? Role_Name { get; set; }
        public string? Company_Name { get; set; }
        public string? Email_Id { get; set; }
        public string? Ip_Address { get; set; }
        public bool Is_Superadmin { get; set; }
        public bool Is_Company_Owner { get; set; }
        public string? API_Token { get; set; }
        public string? Project_Token { get; set; }
        public string? Project_Token_Expiry_Time { get; set; }
        public string? Token_Expiry_Time { get; set; }
        public string? Current_Product_Url { get; set; }
        public string? Previous_Product_Url { get; set; }
        public string? Allowed_Product_Url { get; set; }
        public string? User_Logo { get; set; }
        public string? Login_Type { get; set; }
    }
}
