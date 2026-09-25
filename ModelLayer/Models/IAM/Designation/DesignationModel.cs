using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Designation
{
    public class DesignationRequestModel
    {
        public int Designation_Id { get; set; }
        public int Department_Id { get; set; }
        public string? Designation_Name { get; set; }
        public string? Designation_Code { get; set; }
        public string? Designation_Desc { get; set; }
        public bool Is_Active { get; set; } = true;
    }
    public class DesignationResponseModel
    {
        public int Designation_Id { get; set; }
        public string? ENC_Designation_Id { get; set; }
        public int Department_Id { get; set; }
        public string? Designation_Name { get; set; }
        public string? Department_Name { get; set; }
        public string? Designation_Code { get; set; }
        public string? Designation_Desc { get; set; }
        public bool Is_Active { get; set; }
        public string? Created_On { get; set; }
    }
}
