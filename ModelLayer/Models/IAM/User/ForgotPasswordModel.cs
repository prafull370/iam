using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.IAM.User
{
    
    // Model for initiating Forgot Password request
    public class ForgotPasswordInitiateModel
    {
        public string? Login_Id { get; set; }
        public string? Email_Id { get; set; }
        public string? Mobile_No { get; set; }
    }

    // Model for sending OTP
    public class ForgotPasswordSendOtpModel
    {
        public string? Login_Id { get; set; }
        public int? User_Id { get; set; }
        public string? Email_Id { get; set; }
        public string? Mobile_No { get; set; }
        public int Login_Type_Id { get; set; } // 2 = SMS, 3 = Email
    }

    
    // Model for verifying OTP
    public class ForgotPasswordVerifyOtpModel
    {
        public string? Login_Id { get; set; }
        public int? User_Id { get; set; }
        public string? OTP { get; set; }
    }

    // Model for resetting password
    public class ForgotPasswordResetModel
    {
        public string? Login_Id { get; set; }
        public int? User_Id { get; set; }
        public string? OTP { get; set; }
        public string? New_Password { get; set; }
        public string? Confirm_Password { get; set; }
    }

    
    // Model for OTP verification response
    public class ForgotPasswordOtpVerificationResponse
    {
        public bool Is_Valid { get; set; }
        public bool Is_Expired { get; set; }
        public string? Message { get; set; }
    }
}
