using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Role;

namespace ModelLayer.DataValidators.IAM.Role
{
    public class RoleRequestModelValidator : AbstractValidator<RoleRequestModel>
    {
        public RoleRequestModelValidator()
        {
            RuleFor(x => x.Role_Name).ValidString("Role Name", 0, 20);
            RuleFor(x => x.Role_Desc).ValidStringNotRequired("Description", 0, 200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Role_Id).ValiInt("Role id").When(x => x.Role_Id != 0); // Apply only when ReportId is provided
        }
    }
}
