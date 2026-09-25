using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Designation;

namespace ModelLayer.DataValidators.IAM.Designation
{
    public class DesignationRequestModelValidator : AbstractValidator<DesignationRequestModel>
    {
        public DesignationRequestModelValidator()
        {
            RuleFor(x => x.Department_Id).ValidIntDropdown("department ");
            RuleFor(x => x.Designation_Name).ValidString("Designation Name ", 0, 20);
            RuleFor(x => x.Designation_Code).ValidString("Designation Code ", 0, 10);
            RuleFor(x => x.Designation_Desc).ValidStringNotRequired("Description ",0, 200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Designation_Id).ValiInt("Designation id").When(x => x.Designation_Id != 0); // Apply only when ReportId is provided
        }
    }
}
