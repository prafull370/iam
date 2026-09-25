using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.App;

namespace ModelLayer.DataValidators.IAM.App
{

    public class AppRequestModelValidator : AbstractValidator<AppRequestModel>
    {
        public AppRequestModelValidator()
        {
            RuleFor(x => x.Product_Id).ValidIntDropdown("App Name ");
            RuleFor(x => x.Application_Id).ValidString("Application Id",0,20);
            RuleFor(x => x.Application_Code).ValidString("Application Code ", 0, 20);
            RuleFor(x => x.App_Name).ValidString("App Name ", 0, 20);
            RuleFor(x => x.App_Version).ValidString("App Version ", 0, 20);
            RuleFor(x => x.App_Min_Sdk_Version).ValidString("App Min SDK Version ", 0, 20);
            RuleFor(x => x.App_Target_Sdk_Version).ValidString("App Target Sdk Version ", 0, 20);
            RuleFor(x => x.App_Play_Store_Url).ValidString("App Play Store Url ", 0, 200);
            //RuleFor(x => x.App_Version).ValidString("App Version ", 0, 20);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.App_Id).ValiInt("App id").When(x => x.App_Id != 0); // Apply only when ReportId is provided
        }
    }
}
