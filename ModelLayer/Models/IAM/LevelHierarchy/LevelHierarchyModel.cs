using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;

namespace ModelLayer.Models.IAM.LevelHierarchy
{
	public class LevelHierarchyRequestModel
	{
		public int Level_Id { get; set; }
		public string? Level_Type { get; set; }
		public string? Level_Name { get; set; }
		public string? Level_Code { get; set; }
		public string? Remarks { get; set; }
		public bool Is_Active { get; set; } = true;
		public int Level1_Id { get; set; }
		public int Level2_Id { get; set; }
		public int Level3_Id { get; set; }
		public int Level4_Id { get; set; }
		public int Level5_Id { get; set; }
	}

	public class LevelHierarchyResponseModel
	{
		public int Level_Id { get; set; }
		public string? ENC_Level_Id { get; set; }
		public string? Level_Type { get; set; } = string.Empty;
		public string? Level_Name { get; set; }
		public string? Level_Code { get; set; }
		public string? Remarks { get; set; }
		public string? Created_On { get; set; }
		public bool Is_Active { get; set; } = true;
		public bool Is_Sync { get; set; } = false;
		public int Level1_Id { get; set; }
		public int Level2_Id { get; set; }
		public int Level3_Id { get; set; }
		public int Level4_Id { get; set; }
		public int Level5_Id { get; set; }

	}

	public class LevelHierarchyGetModel
	{
		public int Level_Id { get; set; }
		public string? Level_Type { get; set; }
	}

	public class GetLevelHierarchyModel : PaginationModel
	{
		public string? Level_Type { get; set; }
	}
}
