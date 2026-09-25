using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.ServiceDomainMapping
{
    public class ServiceDomainMappingRequestModel
    {
        public int Discom_Id { get; set; }
		public int Product_Id { get; set; }
        public string? Product_Url { get; set; }
        public string? Product_Api_Url { get; set; }
        public List<ServiceDataModel> Service_Data { get; set; } = new();
    }
    public class ServiceDataModel
    {
        public int Service_Id { get; set; }
		public string? Service_Name { get; set; }
		public string? Service_Url { get; set; }
        public string? Service_Api_Url { get; set; }
		public bool Is_Mapped { get; set; }
	}
    public class ServiceDomainGetModel
    {
        public int Discom_Id { get; set; }
        public int Product_Id { get; set; }
    }
    public class ServiceDomainMappingResponseModel
    {
        public int Discom_Id { get; set; }
        public int Product_Id { get; set; }
        public int Service_Id { get; set; }
        public string? Discom_Name { get; set; }
        public string? Service_Name { get; set; }
        public string? Product_Name { get; set; }
        public string? Service_Logo { get; set; }
        public string? Product_Url { get; set; }
        public string? Product_Api_Url { get; set; }
        public string? Service_Url { get; set; }
        public string? Service_Api_Url { get; set; }
        public bool Is_Microservice { get; set; }
        public bool Is_Mapped { get; set; }
    }
}
