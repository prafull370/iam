using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.UserDeviceMapping;

namespace ModelLayer.DataValidators.IAM.UserDeviceMapping
{
    public class UserDeviceMappingModelRequestModelValidator
        : AbstractValidator<UserDeviceMappingRequestModel>
    {
        public UserDeviceMappingModelRequestModelValidator()
        {
            // User validation
            RuleFor(x => x.User_Id)
                .ValidIntDropdown("user");

            // List validation
            RuleFor(x => x.DeviceMappings)
                .NotNull()
                .WithMessage("Please select at least one application.")
                .Must(x => x.Any())
                .WithMessage("Please select at least one application.");

            // ❌ Duplicate Application not allowed
            RuleFor(x => x.DeviceMappings)
                .Must(list =>
                    list.Select(x => x.ApplicationId?.ToLower())
                        .Distinct()
                        .Count() == list.Count
                )
                .WithMessage("Only one device can be mapped per application.");
        }
    }
    public class UserDeviceMappingItemModelValidator
    : AbstractValidator<UserDeviceMappingItemModel>
    {
        public UserDeviceMappingItemModelValidator()
        {
            RuleFor(x => x.ApplicationId).ValidString("App");
            RuleFor(x => x.DeviceId).ValidString("Device");
        }
    }


}
