using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common;


public class ResponseModel<T>
{
    public bool Status { get; set; } // true for successful requests
    public string? Message { get; set; } // Success or error message
    public T? ResponseData { get; set; } // Data for successful responses
    public string? ResponseType { get; set; } // Optional: ResponseType
    public string? StatusCode { get; set; } // Optional: Status Code For Status 
    public int? RecordId { get; set; } // Optional: Status Code For Status 
    public int TotalRecords { get; set; } // Optional: Status Code For Status 
    public ResponseModel()
    {
        Status = false;
        Message = string.Empty;
        ResponseData = default;
        ResponseType = null;
        StatusCode = null;
        RecordId = 0;
        TotalRecords = 0;
    }
}
