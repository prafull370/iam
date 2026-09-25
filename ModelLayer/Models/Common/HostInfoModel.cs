using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common
{
    public class HostInfoModel
    {
        public int Host_Discom_Id { get; set; }
        public int Host_Product_Id { get; set; }
        public int Host_Service_Id { get; set; }
        public string? Host_Name { get; set; }
        public string? Host_Short_Name { get; set; }
        public string? Host_Url { get; set; }
        public string? Host_API_Url { get; set; }
        public string? Host_Logo { get; set; }
        public string? Source { get; set; }
        
    }
}
