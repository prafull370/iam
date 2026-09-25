using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.DataValidators.IAM.ServiceDomainMapping;
using ModelLayer.Models.IAM.User;

namespace ModelLayer.DataValidators.IAM.User
{
    public class UserRequestModelValidator : AbstractValidator<UserRequestModel>
    {
        public UserRequestModelValidator()
        {
            RuleFor(x => x.User_Name).ValidString("User Name ", 0, 20);
            RuleFor(x => x.Mobile_No).ValidMobileNo("Mobile No ");
            RuleFor(x => x.Email_Id).ValidEmail("Email Id");
            RuleFor(x => x.Join_Date).ValidString("Joining Date ", 0, 15);
            RuleFor(x => x.Start_Date).ValidString("Start Date ", 0, 15);
            RuleFor(x => x.Department_Id).ValidIntDropdown("Department");
            RuleFor(x => x.Designation_Id).ValidIntDropdown("Designation");
            RuleFor(x => x.Vendor_Id).ValidIntDropdown("Vendor").When(x => x.Vendor_Id != null && x.Vendor_Id != 0);
            RuleFor(x => x.Authentication_Type_Id).ValidIntDropdown("Authentication Type");
            RuleFor(x => x.Reporting_User_Id).ValidIntDropdown("Reporting User").When(x => x.Reporting_User_Id != null && x.Reporting_User_Id != 0);
            RuleFor(x => x.Identity_Type_Id).ValidIntDropdown("Identity Type");
            RuleFor(x => x.Identity_No).ValidAadhaarNo(x =>{
                        if (x.Identity_Type_Id == 1) return "aadhaar";
                        if (x.Identity_Type_Id == 2) return "pan";
                        if (x.Identity_Type_Id == 3) return "driving license";
                        return "";}, "Identity No");
            RuleFor(x => x.Login_Id).ValidLoginId("Login Id");
            RuleFor(x => x.Block_Remark_Type_Id).ValidIntDropdown("User Remark").When(x => x.User_Id != 0 && !string.IsNullOrEmpty(x.End_Date));
            RuleForEach(x => x.User_Hierarchy).SetValidator(new UserHierarchyModelValidator());
        }
    }

    public class UserHierarchyModelValidator : AbstractValidator<UserHierarchy>
    {
        public UserHierarchyModelValidator()
        {
            RuleFor(x => x.Discom_Id).ValidIntDropdown("Discom");
        }
    }

    public class UserLoginModelValidator : AbstractValidator<UserLoginModel>
    {
        public UserLoginModelValidator()
        {
            RuleFor(x => x.User_Name).ValidString("UserName", 0, 50);
            RuleFor(x => x.Password).PasswordString("Password", 0, 20);
            RuleFor(x => x.OTP).ValidOTP("OTP");
        }
    }

    public class UserChangePasswordModelValidator : AbstractValidator<UserChangePasswordModel>
    {
        public UserChangePasswordModelValidator()
        {
            RuleFor(x => x.Current_Password).NotEmpty().WithMessage("Current Password is required.");

            RuleFor(x => x.New_Password)
            .NotEmpty().WithMessage("New Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,}$")
            .WithMessage("Password must be at least 8 characters long, alphanumeric, and include at least one uppercase letter and one special character.");

            RuleFor(x => x.Confirm_Password)
                .NotEmpty().WithMessage("Confirm Password is required.")
                .Equal(x => x.New_Password).WithMessage("Passwords do not match.");
        }
    }
}
