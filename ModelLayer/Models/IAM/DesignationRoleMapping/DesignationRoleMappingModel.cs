using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.DesignationRoleMapping
{
    public class DesignationRoleMappingRequestModel
    {
        public int Department_Id { get; set; }
        public int Designation_Id { get; set; }
        public List<int> Role_Ids { get; set; } = new();
    }

    public class DesignationRoleMappingResponseModel
    {
        public int Department_Id { get; set; }
        public int Designation_Id { get; set; }
        public int Role_Id { get; set; }
        public string? Role_Name { get; set; }
        public string? Department_Name { get; set; }
        public string? Designation_Name { get; set; }
        public bool Is_Mapped { get; set; }
    }
    public class DesignationRoleMappingGetModel
    {
        public int Department_Id { get; set; }
        public int Designation_Id { get; set; }
    }
}
