using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Discom;

namespace ModelLayer.DataValidators.IAM.Discom
{
    public class DiscomRequestModelValidator : AbstractValidator<DiscomRequestModel>
    {
        public DiscomRequestModelValidator()
        {
            RuleFor(x => x.Discom_Name).ValidString("Discom Name ", 0, 30);
            RuleFor(x => x.Discom_Short_Code).ValidString("Discom Short Code ", 0, 10);
            RuleFor(x => x.Discom_Name_Regional).ValidStringNotRequired("Discom Name Regional ", 0, 20);
            RuleFor(x => x.Remarks).ValidStringNotRequired("Remarks", 0, 200);
            RuleFor(x => x.Discom_Id).ValiInt("Discom id").When(x => x.Discom_Id != 0); // Apply only when ReportId is provided
        }
    }
}
