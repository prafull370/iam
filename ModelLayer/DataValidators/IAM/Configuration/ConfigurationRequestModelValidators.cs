using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Configuration;

namespace ModelLayer.DataValidators.IAM.Configuration
{
    public class ConfigurationRequestModelValidator : AbstractValidator<ConfigurationRequestModel>
    {
        public ConfigurationRequestModelValidator()
        {
            RuleFor(x => x.Config_Name).ValidString("Configuration Name ", 0, 30);
            RuleFor(x => x.Description).ValidStringNotRequired("Description",0,200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Config_Value).ValidString("Configuration Value", 0, 20);
        }
    }
}