using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Module;

namespace ModelLayer.DataValidators.IAM.Module
{
    public class ModuleRequestModelValidator : AbstractValidator<ModuleRequestModel>
    {
        public ModuleRequestModelValidator()
        {
            RuleFor(x => x.Module_Name).ValidString("Module Name ", 0, 20);
            RuleFor(x => x.Module_Desc).ValidStringNotRequired("Description ", 0, 200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Module_Id).ValiInt("Module id").When(x => x.Module_Id != 0); // Apply only when ReportId is provided
        }
    }
}
