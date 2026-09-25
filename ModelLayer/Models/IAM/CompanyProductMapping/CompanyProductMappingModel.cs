using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.CompanyProductMapping
{
    public class CompanyProductMappingRequestModel
    {
        public int Company_Id { get; set; }
        public List<int> Product_Ids { get; set; } = new();
    }
    public class CompanyProductMappingResponseModel
    {
        public int Company_Id { get; set; }
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Product_Logo { get; set; }
        public bool Is_Mapped { get; set; }
    }
}
