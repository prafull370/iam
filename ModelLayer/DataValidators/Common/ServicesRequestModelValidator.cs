using FluentValidation;

namespace ModelLayer.DataValidaters.Common
{
    public class ServiceRequestModelValidator : AbstractValidator<ServicesRequestModel>
    {
        public ServiceRequestModelValidator()
        {
            RuleFor(x => x.ServiceName).ValidString("Service Name",0,500);
            RuleFor(x => x.ServiceCode).ValidString("Service Code");
            RuleFor(x => x.ServiceLogo).ValidString("Service Logo");
            RuleFor(x => x.Description).ValidString("Description",0,500).When(x=>x.Description!=null);
            RuleFor(x => x.IsActive).ValidBoolean("IsActive");
            RuleFor(x => x.ServiceId).ValidGuid("Service id").When(x => x.ServiceId != Guid.Empty); // Apply only when ReportId is provided
        }
    }

}
