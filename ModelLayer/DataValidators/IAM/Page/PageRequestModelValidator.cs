using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Page;

namespace ModelLayer.DataValidators.IAM.Page
{
    public class PageRequestModelValidator : AbstractValidator<PageRequestModel>
    {
        public PageRequestModelValidator()
        {
            RuleFor(x => x.Product_Id).ValidIntDropdown("Product");
            RuleFor(x => x.Service_Id).ValidIntDropdown("Service");
            //RuleFor(x => x.Parent_Id).ValidIntDropdown("Parent Page");
            //RuleFor(x => x.Parent_Id).LessThan(2147483647).WithMessage("Please select a valid parent page.").When(x => !x.Is_Parent);
            RuleFor(x => x.Parent_Id).Must(id => !id.HasValue || id.Value > 0).WithMessage("Please select a valid parent page.");
            RuleFor(x => x.Access_Id).ValidIntDropdown("Access Point");
            RuleFor(x => x.Page_Name).ValidString("Page Name ", 0, 30);
            //RuleFor(x => x.Page_Url).ValidString("Page Url ",10 , 100);
            RuleFor(x => x.Page_Url).ValidUrlString("Page Url ", 10, 100);
            //RuleFor(x => x.Page_Desc).ValidStringNotRequired("Description ");
            RuleFor(x => x.Page_Desc).ValidDescriptionString("Description ", 0, 200);
            RuleFor(x => x.App_Id).ValidIntDropdown("App");
            RuleFor(x => x.App_Id).LessThan(2147483647).WithMessage("Please select a valid parent page.").When(x => x.Access_Id == 1);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Page_Id).ValiInt("Page id").When(x => x.Page_Id != 0); // Apply only when ReportId is provided
            //RuleFor(x => x.Display_Order).ValiInt(" Display Order").When(x => x.Display_Order != 0).WithMessage("Please Enter a Display Order");
            RuleFor(x => x.Display_Order).GreaterThan(0).WithMessage("Display Order must be greater than 0");
        }
    }
}
