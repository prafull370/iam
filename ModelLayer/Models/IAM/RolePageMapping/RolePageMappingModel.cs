using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.RolePageMapping
{
    public class RolePageMappingRequestModel
    {
        public int Role_Id { get; set; }
        public List<RolePageDataModel> RolePage_Data { get; set; } = new();
    }

    public class RolePageMappingResponseModel
    {
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Product_Logo { get; set; }
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Service_Logo { get; set; }
        public int Module_Id { get; set; }
        public string? Module_Name { get; set; }
        public string? Module_Logo { get; set; }
        
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public string? Page_Logo { get; set; }
		public int Parent_Page_Id { get; set; }
		public string? Parent_Page_Name { get; set; }
		public string? Parent_Page_Logo { get; set; }
		public bool Is_Landing_Page { get; set; }
        public bool Is_Parent { get; set; }
        public bool Is_Product_Mapped { get; set; }
        public bool Is_Service_Mapped { get; set; }
        public bool Is_Module_Mapped { get; set; }
        public bool Is_Page_Mapped { get; set; }
    }

    public class RolePageDataModel
    {
        public int Product_Id { get; set; }
        public int Service_Id { get; set; }
        public int Module_Id { get; set; }
        public int Page_Id { get; set; }
        public bool Is_Landing_Page { get; set; }
    }

    public class PageViewModel
    {
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public string? Page_Logo { get; set; }
        public bool Is_Parent { get; set; }
        public bool Is_Landing_Page { get; set; }
        public bool Is_Page_Mapped { get; set; }
        public int Page_Level { get; set; } = 1; // 1 = root page, up to 5
        public List<PageViewModel> ChildPages { get; set; } = new();
    }

    public class ModuleViewModel
    {
        public int Module_Id { get; set; }
        public string? Module_Name { get; set; }
        public string? Module_Logo { get; set; }
        public List<PageViewModel> ParentPages { get; set; } = new();
    }

    public class ServiceViewModel
    {
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Service_Logo { get; set; }
        public List<ModuleViewModel> Modules { get; set; } = new(); // with module
        public List<PageViewModel> ParentPages { get; set; } = new(); // direct to service (no module)
    }

    public class ProductViewModel
    {
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Product_Logo { get; set; }
        public List<ServiceViewModel> Services { get; set; } = new();
    }

}
