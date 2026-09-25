using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common;

public class DDLRequestModel
{
    public int Id { get; set; }
    public string? Flag { get; set; }
    public string? DDL_Flag { get; set; }
    public int? Exclude_Id { get; set; }
}
public class DDLResponseModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
