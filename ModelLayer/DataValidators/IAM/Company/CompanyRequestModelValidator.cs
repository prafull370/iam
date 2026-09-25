using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Company;

namespace ModelLayer.DataValidators.IAM.Company
{
    public class CompanyRequestModelValidator : AbstractValidator<CompanyRequestModel>
    {
        public CompanyRequestModelValidator()
        {
            RuleFor(x => x.Company_Name).ValidString("Company Name ", 0, 20);
            RuleFor(x => x.Plan_Id).ValiInt("Plan ").When(x=>x.Plan_Id!=0);
            RuleFor(x => x.Company_Short_Name).ValidString("Company Short Name ",0,10);
            RuleFor(x => x.Company_Name_Regional).ValidStringNotRequired("Company Regional Name", 0, 20);
            RuleFor(x => x.Toll_Free_Number).ValidTollFreeNo("Toll Free No ");
            RuleFor(x => x.Company_Website).ValidUrlString("Company Website ", 0, 20);
            RuleFor(x => x.Contact_Person_Name).ValidString("Contact Person Name ", 0, 20);
            RuleFor(x => x.Contact_Email_Id).ValidEmail("Contact Email Id ");
            RuleFor(x => x.Contact_Mobile_No).ValidMobileNo("Contact Mobile No ");
            RuleFor(x => x.Address).ValidStringNotRequired("Address ");
            RuleFor(x => x.Company_Desc).ValidDescriptionString("Description ");
            //RuleFor(x => x.Company_Logo).ValidString("Company Logo ").When(x => !string.IsNullOrWhiteSpace(x.Company_Logo));
            RuleFor(x => x.Cin).ValidCINNo("Cin No ");
            RuleFor(x => x.Tin).ValidTINNo("Tin No ");
            RuleFor(x => x.Gst).ValidGSTNo("Gst No ");
            RuleFor(x => x.Remarks).ValidStringNotRequired("Remark ");
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Company_Id).ValiInt("Company id").When(x => x.Company_Id !=0); // Apply only when ReportId is provided
        }
    }
}
