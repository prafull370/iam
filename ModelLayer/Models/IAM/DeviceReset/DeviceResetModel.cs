
using ModelLayer.Models.Common;

namespace IAM_MODEL.DeviceReset
{
    public class DeviceResetModel
    {
        public int? User_Id { get; set; }
        public string? ENC_Device_Id { get; set; }
        public string? Device_Id { get; set; }
        public string? Login_Id { get; set; }
        public string? User_Name { get; set; }
        public string? Designation_Name { get; set; }
        public string? Department_Name { get; set; }
        public string? Mobile_No { get; set; }
        public int? Total_Records { get; set; } 
    }
    public class ResetDeviceIdFilter : PaginationModel
    {
        public int? User_Id { get; set; }
        public string? ENC_Device_Id { get; set; }
        public string? Device_Id { get; set; }
        public string? Login_Id { get; set; }
        public string? User_Name { get; set; }
        public string? Designation_Name { get; set; }
        public string? Department_Name { get; set; }
        public string? Mobile_No { get; set; }
        public int? Total_Records { get; set; }
    }
    public class DeviceList
    {
        public int? Total_Records { get; set; }
        public List<DeviceResetModel> Device_List { get; set; }
    }
    public class UpdateDeviceId : PaginationModel
    {
        public int? User_Id { get; set; }
        public string? ENC_Device_Id { get; set; }
        public string? Device_Id { get; set; }
        public string? Login_Id { get; set; }
        public string? User_Name { get; set; }
        public string? Designation_Name { get; set; }
        public string? Department_Name { get; set; }
        public string? Mobile_No { get; set; }
        public int? Updated_By { get; set; }
        public string? Ip_Address { get; set; }

    }

    public class DeviceResponseModel
    {
        public int? User_Id { get; set; }
        public string? Device_Id { get; set; }
        public string? Brand_Name { get; set; }
        public string? Application_Id { get; set; }
        public string? ENC_Device_Id { get; set; }
        public string? Device_Name { get; set; }
        public string? Product_Short_Name { get; set; }
        public string? Mac_Address { get; set; }
        public string? Mobile_Android_Version { get; set; }
        public string? Created_On { get; set; }
        public string? Ip_Address { get; set; }
    }
}
