using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.CompanyProductMapping;

namespace ModelLayer.DataValidators.IAM.CompanyProductMapping
{
    public class CompanyProductMappingRequestModelValidator : AbstractValidator<CompanyProductMappingRequestModel>
    {
        public CompanyProductMappingRequestModelValidator()
        {
            RuleFor(x => x.Company_Id).ValidIntDropdown("Company");
            RuleFor(x => x.Product_Ids)
            .NotNull().WithMessage("PLease select product")
            .Must(x => x.Any()).WithMessage("Please select at least one product")
            .Must(list => list.All(id => id > 0)).WithMessage("All product must be greater than 0")
            .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("product must not contain duplicates");
        }
    }
}
