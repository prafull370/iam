using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;

namespace ModelLayer.Models.IAM.ApplicationErrorLog
{
    public class ApplicationErrorLogModel
    {
        public string? S_No { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? ErrorMessage { get; set; }
        public string? LastModified { get; set; }
    }
    public class ApplicationErrorLogListModel
    {
        public List<ApplicationErrorLogModel> Logs { get; set; } = new();
    }
}
