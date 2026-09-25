using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.RolePageMapping;

namespace ModelLayer.DataValidators.IAM.RolePageMapping
{
    public class RolePageMappingRequestModelValidator : AbstractValidator<RolePageMappingRequestModel>
    {
        public RolePageMappingRequestModelValidator()
        {
            RuleFor(x => x.Role_Id).ValidIntDropdown("role.");
            RuleFor(x => x.RolePage_Data).NotNull().WithMessage("PLease select page.")
              .Must(x => x.Any()).WithMessage("Please select at least one page.");
            // No duplicate Service_Id values
            RuleFor(x => x.RolePage_Data)
                .Must(serviceList => serviceList.Select(s => s.Page_Id).Distinct().Count() == serviceList.Count)
                .WithMessage("Duplicate pages are not allowed.");
            //RuleForEach(x => x.RolePage_Data).SetValidator(new RolePageDataModelValidator());
        }
    }
    public class RolePageDataModelValidator : AbstractValidator<RolePageDataModel>
    {
        public RolePageDataModelValidator()
        {
            RuleFor(x => x.Product_Id).ValidIntDropdown("product");
            RuleFor(x => x.Service_Id).ValidIntDropdown("service");
            RuleFor(x => x.Module_Id).ValidIntDropdown("module");
            RuleFor(x => x.Page_Id).ValidIntDropdown("page");
            RuleFor(x => x.Is_Landing_Page).ValidBoolean("Is Landing Page");
        }
    }
}
