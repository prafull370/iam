using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.RolePageAction
{
    public class RolePageActionResponseModel
    {
        public List<PermissionRoleModel>? Roles { get; set; }
        public List<PermissionPageModel>? Pages { get; set; }
        public List<PermissionActionModel>? Actions { get; set; }
        public List<PermissionGrantModel>? Grants { get; set; }
        public List<PageAvailableActionModel>? AvailableActions { get; set; }
    }

    public class PermissionRoleModel
    {
        public int Role_Id { get; set; }
        public string? Role_Name { get; set; }
    }

    public class PermissionPageModel
    {
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public int Module_Id { get; set; }
        public string? Master_Name { get; set; }   // group header on the left ("Master Page")
        public int Display_Order { get; set; }
        public bool? Is_Parent { get; set; }
        public int? Parent_Id { get; set; }
        public bool? Has_Child { get; set; }
        public List<SubMenuModel> subMenus { get; set; }
    }

    public class SubMenuModel
    {
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public int Parent_Id { get; set; }
    }

    public class PermissionActionModel
    {
        public long Action_Id { get; set; }
        public string? Action_Name { get; set; }
        public int Display_Order { get; set; }
        public bool Is_Risky { get; set; }
    }

    public class PageAvailableActionModel
    {
        public int Role_Id { get; set; }
        public int Page_Id { get; set; }
        public long Action_Id { get; set; }
        //public string? Effect { get; set; }         // "A" or "D"
        public bool Is_Allow { get; set; }
        public bool Is_Enabled { get; set; }
    }



    public class PermissionGrantModel
    {
        public int Role_Id { get; set; }
        public int Page_Id { get; set; }
        public long Action_Id { get; set; }
        //public string? Effect { get; set; }
        public bool? Is_Allow { get; set; }
    }

    // ---- Save (request)
    public class RolePageActionRequestModel
    {
        public int Product_Id { get; set; }
        public List<PermissionCellDto> Permissions { get; set; } = new();
    }

    public class PermissionCellDto
    {
        public int Role_Id { get; set; }
        public int Page_Id { get; set; }
        public long Action_Id { get; set; }
        //public string Effect { get; set; } = "A";   // "A" = Allow, "D" = Deny
        public bool Is_Allow { get; set; }   // "A" = Allow, "D" = Deny
    }

}
