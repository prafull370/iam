using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common
{
    public class UserOtpModel
    {
        public string? Login_Id { get; set; }
        public int Login_Type_Id { get; set; }
        public string? Mobile_No { get; set; }
        public string? Email_Id { get; set; }
    }
    public class VerifyOtpModel
    {
        public string? Login_Id { get; set; }
        public string? OTP { get; set; }
    }

    public class GetVerifyOtpResult
    {
        public string? MOBILE_NO { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTP_CREATED_DATE { get; set; }
        public DateTime? OTP_EXPIRY_DATE { get; set; }
    }
    public class EntraLoginRequest 
    { 
        public string Id_Token { get; set; } = "";
    }
}
