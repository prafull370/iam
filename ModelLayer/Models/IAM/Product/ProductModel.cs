using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Product
{
    public class ProductRequestModel
    {
            public int Product_Id { get; set; }
            public string? Product_Name { get; set; }
            public string? Product_Short_Name { get; set; }
            public string? Product_Logo { get; set; }
            public string? Product_Desc { get; set; }
            public bool Is_Active { get; set; } = true;
    }
    public class ProductResponseModel
    {
        public int Product_Id { get; set; }
        public string? ENC_Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Product_Short_Name { get; set; }
        public string? Product_Logo { get; set; }
        public string? Product_Desc { get; set; }
        public bool Is_Active { get; set; }
        public string? Created_On { get; set; }
        public int Display_Order { get; set; }
        public string? Ip_Address { get; set; }
    }
}
