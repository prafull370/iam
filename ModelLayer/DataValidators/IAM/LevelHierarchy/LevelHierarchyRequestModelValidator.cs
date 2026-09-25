using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.LevelHierarchy;

namespace ModelLayer.DataValidators.IAM.LevelHierarchy
{
	public class LevelHierarchyRequestModelValidator : AbstractValidator<LevelHierarchyRequestModel>
	{
		public LevelHierarchyRequestModelValidator()
		{
			RuleFor(x => x.Level_Type).ValidDropdown("Level Type");
			RuleFor(x => x.Level_Name).ValidString("Level Name", 0, 20);
			RuleFor(x => x.Level_Code).ValidString("Level Code", 0, 10);
			RuleFor(x => x.Remarks).ValidStringNotRequired("Remark", 0, 200);
		}
	}
}
