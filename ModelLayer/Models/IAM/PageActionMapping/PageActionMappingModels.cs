using System.Collections.Generic;

namespace ModelLayer.Models.IAM.PageActionMapping
{

    public class PageItemModel
    {
        public int Page_Id { get; set; }
        public string? Page_Name { get; set; }
        public int? Module_Id { get; set; }
        public int? Parent_Id { get; set; }
        public int Display_Order { get; set; }
    }

    public class ActionItemModel
    {
        public long Action_Id { get; set; }
        public string? Action_Name { get; set; }
        public string? Action_Code { get; set; }
        public bool Is_Risky { get; set; }
        public int Display_Order { get; set; }
    }

    // ---- GET mapping
    public class PageActionMappingResponseModel
    {
        public List<ActionItemModel> Actions { get; set; } = new();      // all actions
        public List<long> MappedActionIds { get; set; } = new();         // already mapped -> pre-check
    }

    // used to read the 2nd cursor (mapped action ids)
    public class MappedActionIdModel
    {
        public long Action_Id { get; set; }
    }

    // ---- SAVE (request) --------------------------------------------------
    public class PageActionMappingRequestModel
    {
        public int Product_Id { get; set; }
        public int Service_Id { get; set; }
        public int? Module_Id { get; set; } 
        public int Page_Id { get; set; }
        public List<ActionMapDto> Actions { get; set; } = new();
    }

    public class ActionMapDto
    {
        public long Action_Id { get; set; }
    }

    
}
