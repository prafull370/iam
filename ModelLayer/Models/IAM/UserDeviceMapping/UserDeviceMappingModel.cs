using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.UserDeviceMapping
{
    public class UserDeviceMappingRequestModel
    {
        public int User_Id { get; set; }
        public List<UserDeviceMappingItemModel> DeviceMappings { get; set; } = new();

    }
    public class UserDeviceMappingItemModel
    {
        public int UserId { get; set; }
        public string? ApplicationId { get; set; }
        public string? DeviceId { get; set; }
        public int MappingId { get; set; }
    }

    public class UserDeviceMappingResponseModel
    {
        public int Mapping_Id { get; set; }
        public string? Application_Id { get; set; }
        public string? App_Name { get; set; }
        public string? Device_Id { get; set; }
        public string? Device_Name { get; set; }
        public bool Status { get; set; }
    }
}
