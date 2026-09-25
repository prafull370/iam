using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;

namespace ModelLayer.Models.IAM.ErrorLog
{
    public class DBErrorLogRequestModel:PaginationModel
    {
        public string? FromDate { get; set; }
        public string? EndDate { get; set; }
    }

    public class DBErrorLogResponseModel
    {
        public string? ErrorId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorSource { get; set; }
        public string? ErrorAction { get; set; }
        public string? ErrorType { get; set; }
        public string? ErrorDate { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class ErrorLogModel
    {
        public string? Search { get; set; }
        public string? FromDate { get; set; }
        public string? EndDate { get; set; }
        public string? Message { get; set; }
    }

}
