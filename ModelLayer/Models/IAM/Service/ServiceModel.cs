using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Service
{
    public class ServiceRequestModel
    {
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Service_Desc { get; set; }
        public string? Service_Logo { get; set; }
        public bool Is_Microservice { get; set; } = false;
        public bool Is_Active { get; set; } = true;
        public int? Display_Order { get; set; }
    }
    public class ServiceResponseModel
    {
        public int Service_Id { get; set; }
        public string? ENC_Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Service_Desc { get; set; }
        public string? Service_Logo { get; set; }
        public bool Is_Microservice { get; set; }
        public bool Is_Active { get; set; }
        public string? Created_On { get; set; }
        public int? Display_Order { get; set; }

    }
}
