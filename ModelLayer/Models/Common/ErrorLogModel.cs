using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DataValidaters.Common;

namespace ModelLayer.Models.Common;

public class ErrorLogModel
{
    public int ErrorId { get; set; }
    public string? ErrorType { get; set; }
    public string? ErrorSource { get; set; }
    public string? ErrorAction { get; set; }
    public string? ErrorMessage { get; set; }
}
public class ErrorViewModel
{
    public ErrorLogModel singleerror { get; set; }
    public List<ErrorLogModel> multipleerrors { get; set; }
}
