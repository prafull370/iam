using FluentValidation;
using ModelLayer.Models.IAM.PageActionMapping;

namespace ModelLayer.DataValidaters.IAM.PageActionMapping
{
    // Auto-registered via AddValidatorsFromAssemblyContaining (ModelLayer).
    public class PageActionMappingRequestModelValidator : AbstractValidator<PageActionMappingRequestModel>
    {
        public PageActionMappingRequestModelValidator()
        {
            RuleFor(x => x.Product_Id).GreaterThan(0).WithMessage("Product is required.");
            RuleFor(x => x.Service_Id).GreaterThan(0).WithMessage("Service is required.");
            RuleFor(x => x.Page_Id).GreaterThan(0).WithMessage("Page is required.");

            RuleFor(x => x.Actions)
                .NotNull().Must(a => a != null && a.Count > 0)
                .WithMessage("At least one action must be selected.");

            // Prevent duplicate action ids inside a single payload.
            RuleFor(x => x.Actions)
                .Must(a => a == null || a.Select(i => i.Action_Id).Distinct().Count() == a.Count)
                .WithMessage("Duplicate actions are not allowed in the mapping.");
        }
    }
}
