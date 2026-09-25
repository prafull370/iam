using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.ProductServiceMapping
{
    public class ProductServiceMappingRequestModel
    {
        public int Product_Id { get; set; }
        public List<int> Service_Ids { get; set; } = new();
    }
    public class ProductServiceMappingResponseModel
    {
        public int Product_Id { get; set; }
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Product_Name { get; set; }
        public string? Service_Logo { get; set; }
        public bool Is_Microservice { get; set; }
        public bool Is_Mapped { get; set; }
    }
}
