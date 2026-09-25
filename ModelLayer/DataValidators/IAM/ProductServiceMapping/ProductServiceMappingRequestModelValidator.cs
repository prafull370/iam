using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.ProductServiceMapping;

namespace ModelLayer.DataValidators.IAM.ProductServiceMapping
{
    public class ProductServiceMappingRequestModelValidator : AbstractValidator<ProductServiceMappingRequestModel>
    {
        public ProductServiceMappingRequestModelValidator()
        {
            RuleFor(x => x.Product_Id).ValidIntDropdown("product");
            RuleFor(x => x.Service_Ids)
            .NotNull().WithMessage("PLease select service")
            .Must(x => x.Any()).WithMessage("Please select at least one service")
            .Must(list => list.All(id => id > 0)).WithMessage("All service must be greater than 0")
            .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("service must not contain duplicates");
        }
    }
}
