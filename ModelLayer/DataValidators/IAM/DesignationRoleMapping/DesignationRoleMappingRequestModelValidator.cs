using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;

namespace ModelLayer.DataValidators.IAM.DesignationRoleMapping
{
    public class DesignationRoleMappingRequestModelValidator : AbstractValidator<DesignationRoleMappingRequestModel>
    {
        public DesignationRoleMappingRequestModelValidator()
        {
            RuleFor(x => x.Department_Id).ValidIntDropdown("department");
            RuleFor(x => x.Designation_Id).ValidIntDropdown("designation");
            RuleFor(x => x.Role_Ids)
            .NotNull().WithMessage("Please select role")
            .Must(x => x.Any()).WithMessage("At least one role must be selected")
            .Must(list => list.All(id => id > 0)).WithMessage("All role must be greater than 0")
            .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("role must not contain duplicates");
        }
    }
}
