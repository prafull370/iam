using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Dashboard
{
    public class DashboardResponseModel
    {
        public int Total_Users { get; set; }
        public int Active_Users { get; set; }
        public int Users_This_Month { get; set; }
        public int Users_Last_Month { get; set; }
        public int Total_Discoms { get; set; }
        public int Active_Discoms { get; set; }
        public int Total_Products { get; set; }
        public int Active_Products { get; set; }
        public int Total_Roles { get; set; }
        public int Active_Roles { get; set; }
    }
    public class DashboardRoleDistributionResponseModel
    {
        public string? Role_Name { get; set; }
        public int User_Count { get; set; }
        public int Total_Roles { get; set; }
    }

    public class DashboardSecurityResponseModel
    {
        public int Failed_Attempt_Users { get; set; }
        public int Locked_Accounts { get; set; }
        public int New_Devices_Today { get; set; }
        //public int Errors_Today { get; set; }
        public int Active_Sessions { get; set; }
    }

    public class DashboardRecentActivityResponseModel
    {
        public int User_Id { get; set; }
        public string? User_Name { get; set; }
        public string? Activity_Type { get; set; }
        public string? Page_Name { get; set; }
        public string? Description { get; set; }
        public string? Ip_Address { get; set; }
        public string? Created_On { get; set; }
        public string? Status { get; set; }
    }

    public class DashboardLoginTrendResponseModel
    {
        public string? Day_Label { get; set; }
        public int Login_Count { get; set; }
    }
}
