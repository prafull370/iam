using ModelLayer.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.AppAuditLog
{
    public class AppAuditLogModel
    {
    }
    public class AppAuditLogRequestModel : PaginationModel
    {
        public string? From_Date { get; set; }
        public string? To_Date { get; set; }
    }

    // One row per app user (main grid)
    public class AppAuditUserResponseModel
    {
        public string? Login_Id { get; set; }
        public string? User_Name { get; set; }
        public string? Mobile_No { get; set; }
        public string? Brand_Name { get; set; }
        public string? Device_Name { get; set; }
        public string? Mobile_Android_Version { get; set; }
        public string? App_Version { get; set; }
        public string? Last_Activity { get; set; }
        public int Total_Actions { get; set; }
    }

    // One row per audit action (drawer)
    public class AppAuditDetailResponseModel
    {
        public int Audit_Id { get; set; }
        public string? Login_Id { get; set; }
        public string? Brand_Name { get; set; }
        public string? Device_Name { get; set; }
        public string? Device_Id { get; set; }
        public string? Mobile_Android_Version { get; set; }
        public string? Sdk_Int { get; set; }
        public string? App_Version { get; set; }
        public string? Audit_App_Release { get; set; }
        public string? Application_Id { get; set; }
        public string? Audit_Application_Id { get; set; }
        public string? Request_Url { get; set; }
        public string? Audit_Response { get; set; }
        public string? Mac_Address { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Created_On { get; set; }
    }

    // Summary cards
    public class AppAuditStatsResponseModel
    {
        public int Total_Users { get; set; }
        public int Events_Today { get; set; }
        public int Active_Users_Today { get; set; }
        public int Total_Devices { get; set; }
    }
}
