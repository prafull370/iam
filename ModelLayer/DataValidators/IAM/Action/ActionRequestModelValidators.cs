using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.DataValidators.IAM.Action
{
    public class ActionRequestModelValidator : AbstractValidator<ActionRequestModel>
    {
        public ActionRequestModelValidator()
        {
            RuleFor(x => x.Action_Name).ValidString("Action Name ", 0, 20);
            RuleFor(x => x.Action_Code).ValidString("Action Code ", 0, 10);
            RuleFor(x => x.Action_Desc).ValidStringNotRequired("Description ", 0 , 200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Action_Id).ValiInt("Action id").When(x => x.Action_Id != 0);
        }
    }
}
