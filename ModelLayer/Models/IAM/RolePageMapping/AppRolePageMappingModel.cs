using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.RolePageMapping
{
    public class AppRolePageMappingRequestModel
    {
        public int Role_Id { get; set; }
        public List<AppRolePageDataModel> RolePage_Data { get; set; } = new();
    }

    public class AppRolePageMappingResponseModel
    {
        public int App_Id { get; set; }
        public string? App_Name { get; set; }
        public string? App_Logo { get; set; }
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Service_Logo { get; set; }
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public string? Page_Logo { get; set; }
		public int Parent_Page_Id { get; set; }
		public string? Parent_Page_Name { get; set; }
		public string? Parent_Page_Logo { get; set; }
		public bool Is_Landing_Page { get; set; }
        public bool Is_Parent { get; set; }
        public bool Is_App_Mapped { get; set; }
        public bool Is_Service_Mapped { get; set; }
        public bool Is_Page_Mapped { get; set; }
    }

    public class AppRolePageDataModel
    {
        public int App_Id { get; set; }
        public int Service_Id { get; set; }
        public int Page_Id { get; set; }
        public bool Is_Landing_Page { get; set; }
    }

    public class AppPageViewModel
    {
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public string? Page_Logo { get; set; }
        public bool Is_Parent { get; set; }
        public bool Is_Landing_Page { get; set; }
        public bool Is_Page_Mapped { get; set; }
        public List<AppPageViewModel> ChildPages { get; set; } = new();
    }
    public class AppServiceViewModel
    {
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public string? Service_Logo { get; set; }
        public List<AppPageViewModel> ParentPages { get; set; } = new(); // direct to service (no module)
    }

    public class AppViewModel
    {
        public int App_Id { get; set; }
        public string? App_Name { get; set; }
        public string? App_Logo { get; set; }
        public List<AppServiceViewModel> Services { get; set; } = new();
    }

}
