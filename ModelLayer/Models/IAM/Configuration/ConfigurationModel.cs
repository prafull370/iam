using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Configuration
{
    public class ConfigurationRequestModel
    {
        public int Config_Id { get; set; }
        public string? Config_Name { get; set; }
        public string? Config_Value { get; set; }
        public string? Description { get; set; }
        public bool Is_Active { get; set; } = true;
        public string? Is_Config_Value { get; set; }
    }
    public class ConfigurationResponseModel
    {
        public int Config_Id { get; set; }
        public string? ENC_Config_Id { get; set; }
        public string? Config_Name { get; set; }
        public string? Config_Value { get; set; }
        public string? Description { get; set; }
        public string? Created_On { get; set; }
        public bool Is_Active { get; set; }
        public string? Is_Config_Value { get; set; }
    }
}
