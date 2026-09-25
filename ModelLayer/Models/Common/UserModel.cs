using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common
{
    public class UserModel
    {
       public int? Id { get; set; }  
       public string? Name { get; set; }
       public string? Token { get; set; }
    }
}
