using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.App;

namespace ModelLayer.DataValidators.Common
{
    public class DeviceRegistrationRequestModelValidator : AbstractValidator<AppDeviceRegistrationRequestModel>
    {
        public DeviceRegistrationRequestModelValidator()
        {
            RuleFor(x => x.Login_Id).ValidString("Login id ");
            RuleFor(x => x.Password).ValidString("Password", 5, 50);
            RuleFor(x => x.Application_Id).ValidString("Application Id", 0, 50);
            RuleFor(x => x.Audit.Device_Id).ValidString("Device Id", 0, 50);
            }
    }
}
