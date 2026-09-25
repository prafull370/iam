using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Department
{
    public class DepartmentRequestModel
    {
        public int Department_Id { get; set; }
        public string? Department_Name { get; set; }
        public string? Department_Code { get; set; }
        public string? Department_Desc { get; set; }
        public bool Is_Active { get; set; } = true;
    }
    public class DepartmentResponseModel
    {
        public int Department_Id { get; set; }
        public string? ENC_Department_Id { get; set; }
        public string? Department_Name { get; set; }
        public string? Department_Code { get; set; }
        public string? Department_Desc { get; set; }
        public string? Created_On { get; set; }
        public bool Is_Active { get; set; }
    }
}
