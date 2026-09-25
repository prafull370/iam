using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Department;

namespace ModelLayer.DataValidators.IAM.Department
{
    public class DepartmentRequestModelValidator : AbstractValidator<DepartmentRequestModel>
    {
        public DepartmentRequestModelValidator()
        {
            RuleFor(x => x.Department_Name).ValidString("Department Name ", 0, 20);
            RuleFor(x => x.Department_Code).ValidString("Department Code ", 0, 10);
            RuleFor(x => x.Department_Desc).ValidStringNotRequired("Description",0 , 200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Department_Id).ValiInt("Department id").When(x => x.Department_Id != 0); // Apply only when ReportId is provided
        }
    }
}