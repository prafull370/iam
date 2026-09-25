using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.VersionControl
{
    public class VersionControlRequestModel
    {
        public int? Version_Id { get; set; }
        public int? Product_Id { get; set; }
        public int? Version_Point { get; set; }
        public int? App_Id { get; set; }
        //public string? Application_Id { get; set; }
        //public string? Version_For { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public DateTime? Created_On { get; set; }
        public bool Is_Active { get; set; } = true;
        public string? Description { get; set; }

    }
    public class VersionControlResponseModel
    {
        public int? Version_Id { get; set; }
        public string? ENC_Version_Id { get; set; }
        public int? Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Application_Id { get; set; }
        public string? Application_Code { get; set; }
        public string? Version_For { get; set; }
        public string? Version { get; set; }
        public bool Is_Active { get; set; } = true;
        public string? Created_On { get; set; }
        public int? App_Id { get; set; }
        public string? Logo { get; set; }
        public string? Description { get; set; }
    }
}
