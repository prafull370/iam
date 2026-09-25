using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Role
{
    public class RoleRequestModel
    {
        public int Role_Id { get; set; }
        public string? Role_Name { get; set; }
        public string? Role_Desc { get; set; }
        public bool Is_Active { get; set; } = true;
        public string? Created_On { get; set; }
    }
    public class RoleResponseModel
    {
        public int Role_Id { get; set; }
        public string? ENC_Role_Id { get; set; }
        public string? Role_Name { get; set; }
        public string? Role_Desc { get; set; }
        public bool Is_Active { get; set; }
		public string? Created_On { get; set; }
	}
}
