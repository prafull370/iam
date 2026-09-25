using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.DiscomProductMapping;

namespace ModelLayer.DataValidators.IAM.DicomProductMappingProductMapping
{
    public class DicomProductMappingRequestModelValidator : AbstractValidator<DiscomProductMappingRequestModel>
    {
        public DicomProductMappingRequestModelValidator()
        {
            RuleFor(x => x.Discom_Id).ValidIntDropdown("discom");
            RuleFor(x => x.Product_Ids)
            .NotNull().WithMessage("Product_Ids is required")
            .Must(x => x.Any()).WithMessage("Please select at least one product")
            .Must(list => list.All(id => id > 0)).WithMessage("All product must be greater than 0")
            .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("product must not contain duplicates");
        }
    }
}
