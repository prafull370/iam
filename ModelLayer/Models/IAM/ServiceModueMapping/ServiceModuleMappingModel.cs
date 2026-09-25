using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.ServiceModuleMapping
{
    public class ServiceModuleMappingRequestModel
    {
        public int Service_Id { get; set; }
        public List<int> Module_Ids { get; set; } = new();
    }
    public class ServiceModuleMappingResponseModel
    {
        public int Service_Id { get; set; }
        public int Module_Id { get; set; }
        public string? Module_Name { get; set; }
        public string? Service_Name { get; set; }
        public string? Module_Logo { get; set; }
        public bool Is_Mapped { get; set; }
    }
}
