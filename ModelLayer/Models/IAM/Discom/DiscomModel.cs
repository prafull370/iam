using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Discom
{
    public class DiscomRequestModel
    {
        public int Discom_Id { get; set; }
        public string? Discom_Name { get; set; }
        public string? Discom_Short_Code { get; set; }
        public string? Discom_Name_Regional { get; set; }
        public string? Discom_Logo { get; set; }
        public string? Remarks { get; set; }
        public bool Is_Active { get; set; } = true;
    }
    public class DiscomResponseModel
    {
        public int Discom_Id { get; set; }
        public string? ENC_Discom_Id { get; set; }
        public string? Discom_Name { get; set; }
        public string? Discom_Short_Code { get; set; }
        public string? Discom_Name_Regional { get; set; }
        public string? Discom_Logo { get; set; }
        public string? Remarks { get; set; }
        public string? Created_On { get; set; }
        public string? Display_Order { get; set; }
        public bool Is_Active { get; set; }
    }
}
