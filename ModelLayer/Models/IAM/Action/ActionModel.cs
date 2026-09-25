using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.Action
{
    public class ActionRequestModel
    {
        public int Action_Id { get; set; }
        public string? Action_Name { get; set; }
        public string? Action_Code { get; set; }
        public string? Action_Desc { get; set; }
        public bool Is_Active { get; set; } = true;
        public string? Created_On { get; set; }
        public string? Old_Data_Json { get; set; }
        public string? New_Data_Json { get; set; }
    }
    public class ActionResponseModel
    {
        public int Action_Id { get; set; }
        public string? Action_Name { get; set; }
        public string? Action_Code { get; set; }
        public string? Action_Desc { get; set; }
        public bool Is_Active { get; set; }
		public string? Created_On { get; set; }
        public string? ENC_Action_Id { get; set; }
    }
}
