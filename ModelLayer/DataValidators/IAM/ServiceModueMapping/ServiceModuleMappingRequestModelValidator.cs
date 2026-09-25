using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.ServiceModuleMapping;

namespace ModelLayer.DataValidators.IAM.ServiceModueMapping
{
    public class ServiceModuleMappingRequestModelValidator : AbstractValidator<ServiceModuleMappingRequestModel>
    {
        public ServiceModuleMappingRequestModelValidator()
        {
            RuleFor(x => x.Service_Id).ValidIntDropdown("service.");
            RuleFor(x => x.Module_Ids)
            .NotNull().WithMessage("PLease select module")
            .Must(x => x.Any()).WithMessage("Please select at least one module")
            .Must(list => list.All(id => id > 0)).WithMessage("All module must be greater than 0")
            .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("module must not contain duplicates");
        }
    }
}
