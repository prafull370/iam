using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common
{
    public class CommonFieldModel
    {
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
