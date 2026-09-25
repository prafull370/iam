using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common
{
    public class PaginationModel
    {
        public int Start { get; set; } = 0;
        public int Length { get; set; } = 15;
        public string SortColumn { get; set; } = "created_on";
        public string SortColumnDirection { get; set; } = "DESC";
        public string Flag { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string Search_Val { get; set; } = string.Empty;
    }
    
}
