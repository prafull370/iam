using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.App
{
    public class AppRequestModel
    {
        public int App_Id { get; set; }
        public int Product_Id { get; set; }
        public string? Application_Id { get; set; }
        public string? Application_Code { get; set; }
        public string? App_Name { get; set; }
        public string? App_Version { get; set; }
        public string? App_Min_Sdk_Version { get; set; }
        public string? App_Target_Sdk_Version { get; set; }
        public string? App_Play_Store_Url { get; set; }
        public string? App_Logo { get; set; }
        public bool Is_Active { get; set; } = true;
    }
    public class AppResponseModel
    {
        public int App_Id { get; set; }
        public string? ENC_App_Id { get; set; }
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Application_Id { get; set; }
        public string? Application_Code { get; set; }
        public string? App_Name { get; set; }
        public string? App_Version { get; set; }
        public string? App_Min_Sdk_Version { get; set; }
        public string? App_Target_Sdk_Version { get; set; }
        public string? App_Play_Store_Url { get; set; }
        public string? App_Logo { get; set; }
        public bool Is_Active { get; set; } = true;
        public string? Created_On { get; set; }
    }
}
