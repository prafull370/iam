using ModelLayer.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Page
{
    public class PageRequestModel
    {
        public int Page_Id { get; set; }
        public int Access_Id { get; set; }
        public int App_Id { get; set; }
        public int Product_Id { get; set; }
        public int Service_Id { get; set; }
        public int Module_Id { get; set; } = 0;
        public int? Parent_Id { get; set; }
        public string? Page_Name { get; set; }
        public string? Page_Url { get; set; }
        public string? Page_Desc { get; set; }
        public string? Page_Logo { get; set; }
        public bool Is_Active { get; set; } = true;
        public bool Is_Parent { get; set; } = false;
        public int? Display_Order { get; set; }
        public bool Activity_Log { get; set; }
    }
    public class PageResponseModel
    {
        public int Page_Id { get; set; }
        public string? ENC_Page_Id { get; set; }
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public int Service_Id { get; set; }
        public string? Service_Name { get; set; }
        public int? Module_Id { get; set; }
        public string? Module_Name { get; set; }
        public int? Parent_Id { get; set; }
        public string? Parent_Page_Name { get; set; }
        public int Access_Id { get; set; }
        public int App_Id { get; set; }
        public string? App_Name { get; set; }
        public string? Page_Name { get; set; }
        public string? Page_Url { get; set; }
        public string? Page_Desc { get; set; }
        public string? Page_Logo { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Parent { get; set; }
        public int Display_Order { get; set; }
        public string? Created_On { get; set; }
        public bool Activity_Log { get; set; }
    }
    public class PageGetRequestModel : PaginationModel
    {
        public int? Product_Id { get; set; }
        public int? Service_Id { get; set; }
        public int? Access_Id { get; set; } 
    }

}
