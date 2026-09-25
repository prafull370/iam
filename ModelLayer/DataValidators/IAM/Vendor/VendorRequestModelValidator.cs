using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Vendor;

namespace ModelLayer.DataValidators.IAM.Vendor
{
    public class VendorRequestModelValidator : AbstractValidator<VendorRequestModel>
    {
        public VendorRequestModelValidator()
        {
            RuleFor(x => x.Vendor_Name).ValidString("Vendor Name ", 0, 20);
            RuleFor(x => x.Country_Id).ValidIntDropdown("country");
            RuleFor(x => x.State_Id).ValidIntDropdown("state");
            RuleFor(x => x.District_Id).ValidIntDropdown("district");
            RuleFor(x => x.City_Name).ValidString("City",1,50);
            RuleFor(x => x.Pin_Code).ValidString("Pincode",6,6);
            RuleFor(x => x.Address1).ValidString("Address 1 ");
            RuleFor(x => x.Address2).ValidStringNotRequired("Description ");
            RuleFor(x => x.Vendor_Desc).ValidStringNotRequired("Description ", 0, 200);
            RuleFor(x => x.Contact_Person_Name).ValidString("Contact Person Name ",3,20);
            RuleFor(x => x.Contact_Person_Email_Id).ValidEmail("Contact Person Email ");
            RuleFor(x => x.Contact_Person_Mobile_No).ValidMobileNo("Contact Person Mobile No ");
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Vendor_Id).ValiInt("Vendor id").When(x => x.Vendor_Id != 0); // Apply only when ReportId is provided
        }
    }
}
