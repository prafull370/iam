using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Module
{
    public class ModuleRequestModel
    {
        public int Module_Id { get; set; }
        public string? Module_Name { get; set; }
        public string? Module_Logo { get; set; }
        public string? Module_Desc { get; set; }
        public bool Is_Active { get; set; } = true;
        // add new 
        //public IFormFile POSTED_FILE { get; set; }
    }
    public class ModuleResponseModel
    {
        public int Module_Id { get; set; }
        public string? ENC_Module_Id { get; set; }
        public string? Module_Name { get; set; }
        public string? Module_Logo { get; set; }
        public string? Module_Desc { get; set; }
        public string? Created_On { get; set; }
        public bool Is_Active { get; set; }
    }
}
