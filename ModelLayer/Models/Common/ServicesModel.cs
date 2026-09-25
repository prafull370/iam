using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.DataValidaters.Common
{
    public class ServicesResponseModel
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? ServiceCode { get; set; }
        public string? ServiceLogo { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class ServicesRequestModel
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? ServiceCode { get; set; }
        public string? ServiceLogo { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
