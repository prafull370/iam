using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;


namespace ModelLayer.DataValidaters.Common
{
    public static class CommonValidationRules
    {// Regex pattern to detect common SQL injection attempts
        private static readonly Regex SqlInjectionRegex = new Regex(
        @"\b(select\s+[\w\*,\s]+\s+from|delete\s+from|insert\s+into|update\s+\w+\s+set|drop\s+database|drop\s+table|drop\s+view|drop\s+procedure|truncate\s+table|alter\s+table|alter\s+procedure|alter\s+view|exec\s+\w+|union\s+select|xp_cmdshell|sp_.*|--|\/\*|\*\/)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Special Character Regex
        private static readonly Regex SpecialCharRegex = new Regex(@"^[a-zA-Z0-9@.\-&_]+( [a-zA-Z0-9@.\-&_]+)*$", RegexOptions.Compiled);

        // password regex 
        private static readonly Regex PasswordRegex = new Regex(@"^[a-zA-Z0-9@.\-#!$%&^]+( [a-zA-Z0-9@.\-#!$%&^]+)*$", RegexOptions.Compiled);
        // url Regex
        private static readonly Regex UrlRegex = new Regex(
            @"^(" +
            @"(https?:\/\/" +                                      // full URL start
            @"((localhost(:\d{1,5})?)|" +
            @"((\d{1,3}\.){3}\d{1,3}(:\d{1,5})?)|" +
            @"(([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(:\d{1,5})?))" +
            @"(\/[^\s]*)?" +
            @")" +
            @"|" +
            @"\/?[a-zA-Z0-9_\-\/\.]+(\?[a-zA-Z0-9\-._~%!$&'()*+,;=:@\/?]*)?" +  // relative url like CS/Dashboard/Index
            @")$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // String Validation Rule
        public static IRuleBuilderOptions<T, string?> ValidString<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName, int minLength = 0, int maxLength = 250)
        {
            var rule = ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required")
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage($"{fieldName} is required")
                .MaximumLength(maxLength).WithMessage($"{fieldName} cannot exceed {maxLength} characters")
                .MinimumLength(minLength).WithMessage($"{fieldName} must be at least {minLength} characters long")
                .Must(value => !SqlInjectionRegex.IsMatch(value == null ? "" : value))
                .WithMessage($"{fieldName} contains invalid characters or SQL injection patterns.")
                .Must(value => SpecialCharRegex.IsMatch(value ?? "")).WithMessage($"{fieldName} contains invalid characters or special characters");
            return rule;
        }
        // String Validation Rule
        public static IRuleBuilderOptions<T, string?> ValidStringNotRequired<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName, int minLength = 0, int maxLength = 250)
        {
            var rule = ruleBuilder
                .MaximumLength(maxLength).WithMessage($"{fieldName} cannot exceed {maxLength} characters")
                .MinimumLength(minLength).WithMessage($"{fieldName} must be at least {minLength} characters long")
                .Must(value => !SqlInjectionRegex.IsMatch(value == null ? "" : value))
                .WithMessage($"{fieldName} contains invalid characters or SQL injection patterns.");
            return rule;
        }
        // GST NO Validation Rule
        public static IRuleBuilderOptions<T, string?> ValidGSTNo<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .Matches(@"^$|^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
                    .WithMessage($"{fieldName} format is invalid (e.g., 22AAAAA0000A1Z5)");
        }

        // CIN NO Validation Rule
        public static IRuleBuilderOptions<T, string> ValidCINNo<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .Matches(@"^$|^[A-Z]{1}[0-9]{5}[A-Z]{2}[0-9]{4}[A-Z]{3}[0-9]{6}$")
                .WithMessage($"{fieldName} format is invalid (e.g., U12345MH2000PLC123456)");
        }

        // TIN NO Validation Rule
        public static IRuleBuilderOptions<T, string> ValidTINNo<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .Matches(@"^$|^[0-9]{11}$")
                .WithMessage($"{fieldName} must be exactly 11 digits (e.g., 12345678901)");
        }


        // Int Validation Rule
        public static IRuleBuilderOptions<T, int> ValiInt<T>(this IRuleBuilder<T, int> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .InclusiveBetween(1, int.MaxValue)
                .WithMessage($"{fieldName} is required.");
        }
        public static IRuleBuilderOptions<T, int?> ValiInt<T>(this IRuleBuilder<T, int?> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required.")
                .InclusiveBetween(1, int.MaxValue)
                .WithMessage($"{fieldName} is required.");
        }
        // Int Validation Rule
        public static IRuleBuilderOptions<T, int> ValidIntDropdown<T>(this IRuleBuilder<T, int> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .NotEmpty().WithMessage($"Please select {fieldName}")
                .NotNull().WithMessage($"Please select {fieldName}")
                .InclusiveBetween(1, int.MaxValue)
                .WithMessage($"Please select {fieldName}");
        }
        public static IRuleBuilderOptions<T, string?> ValidOTP<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName = "OTP")
        {
            return ruleBuilder
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Matches(@"^[0-9]{6}$")
                .WithMessage($"{fieldName} must be exactly 6 digits");
        }

        // Int Validation Rule
        public static IRuleBuilderOptions<T, int?> ValidIntDropdown<T>(this IRuleBuilder<T, int?> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .NotEmpty().WithMessage($"Please select {fieldName}")
                .NotNull().WithMessage($"Please select {fieldName}")
                .InclusiveBetween(1, int.MaxValue)
                .WithMessage($"Please select {fieldName}");
        }

        // Dropdown Validation Rule
        public static IRuleBuilderOptions<T, string> ValidDropdown<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName)
        {
            return ruleBuilder.NotNull().WithMessage($"Please select {fieldName}")
                .NotEmpty().WithMessage($"Please select {fieldName}");
        }

        // ✅ Validate Integer Length (by converting it to a string)
        public static IRuleBuilderOptions<T, int> ValidIntWithLength<T>(
            this IRuleBuilder<T, int> ruleBuilder, string fieldName, int minLength = 1, int maxLength = 10)
        {
            return ruleBuilder
                .Must(value => value.ToString().Length >= minLength && value.ToString().Length <= maxLength)
                .WithMessage($"{fieldName} must be between {minLength} and {maxLength} digits long.");
        }

        // ✅ Validate Integer Value Range
        public static IRuleBuilderOptions<T, int> ValidIntWithValue<T>(
            this IRuleBuilder<T, int> ruleBuilder, string fieldName, int minValue = 0, int maxValue = 10)
        {
            return ruleBuilder
                .InclusiveBetween(minValue, maxValue)
                .WithMessage($"{fieldName} must be between {minValue} and {maxValue}.");
        }
        // GUID Validation Rule
        public static IRuleBuilderOptions<T, Guid> ValidGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required")
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Must(guid => guid != Guid.Empty).WithMessage($"{fieldName} cannot be an empty GUID");
        }
        // Boolean Validation Rule
        public static IRuleBuilderOptions<T, bool> ValidBoolean<T>(this IRuleBuilder<T, bool> ruleBuilder, string fieldName)
        {
            return ruleBuilder
                .Must(value => value == true || value == false)
                .WithMessage($"{fieldName} must be either true or false");
        }
        // Email Validation Rule
        public static IRuleBuilderOptions<T, string?> ValidEmail<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName = "Email")
        {
            return ruleBuilder
             .NotNull().WithMessage($"{fieldName} is required")
             .NotEmpty().WithMessage($"{fieldName} is required")
             .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
             .WithMessage($"Invalid {fieldName}. Enter a valid email address.");
        }
        // Mobile No Validation Rule
        public static IRuleBuilderOptions<T, string?> ValidMobileNo<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName = "Mobile No")
        {
            return ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required")
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Matches(@"^(\+91[-\s]?)?[6789]\d{9}$") // Indian Mobile Number Regex
                .WithMessage($"Invalid {fieldName}. It must be a valid Indian mobile number.");
        }
        // Toll Free No Validation Rule
        public static IRuleBuilderOptions<T, string> ValidTollFreeNo<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName = "Toll-Free No")
        {
            return ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required")
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Matches(@"^(1800|1860)[-\s]?\d{3}[-\s]?\d{4}$")
                .WithMessage($"Invalid {fieldName}. It must start with 1800 or 1860 and be followed by 7 digits.");
        }
        public static IRuleBuilderOptions<T, string?> ValidAadhaarNo<T>(this IRuleBuilder<T, string?> ruleBuilder,Func<T, string?> identityTypeSelector, string fieldName = "Identity No")
        {
            return ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required")
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Must((model, identityNo) =>{
                    if (string.IsNullOrWhiteSpace(identityNo)) return false;
                    var type = identityTypeSelector(model)?.ToLower();
                    identityNo = identityNo.Trim().ToUpper();
                    //  Aadhaar validation
                    if (type == "aadhaar" || type == "1"){
                        return Regex.IsMatch(identityNo, @"^\d{12}$");
                    }
                    //  PAN validation
                    if (type == "pan" || type == "2"){
                        var panRegex = new Regex(@"^[A-Z]{5}[0-9]{4}[A-Z]$");
                        if (!panRegex.IsMatch(identityNo)) return false;
                        char fourth = identityNo[3];
                        char[] allowed = { 'P', 'C', 'H', 'A', 'B', 'G', 'J', 'L', 'F', 'T' };
                        return allowed.Contains(fourth);
                    }
                    //  Driving License validation
                    if (type == "driving license" || type == "3")
                    {
                        return Regex.IsMatch(identityNo, @"^[A-Z]{2}[0-9]{2}[0-9]{4}[0-9]{7}$");
                    }
                    return true; })
                .WithMessage(model =>
                {
                    var type = identityTypeSelector(model)?.ToLower();
                    if (type == "pan") return "Invalid PAN number.Ex: IKPPL1504L";
                    if (type == "aadhaar") return "Invalid Aadhaar number. It must be 12 digits.";
                    if (type == "driving license") return "Invalid Driving License number.Ex: UP1420227654321";
                    return "Invalid identity number.";
                });}

        // Validate Login Id
        public static IRuleBuilderOptions<T, string?> ValidLoginId<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName = "Login ID")
        {
            return ruleBuilder
                .Matches(@"^(?:[a-zA-Z0-9.]{3,20}|[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$")
                .WithMessage($"Invalid {fieldName}. It must be a valid email OR a username (letters, numbers, dot, 3–20 chars).");
        }

        // URL Validation Rule 

        public static IRuleBuilderOptions<T, string?> ValidUrlString<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName, int minLength = 0, int maxLength = 250)
        {
            var rule = ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required")
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage($"{fieldName} is required")
                .MaximumLength(maxLength).WithMessage($"{fieldName} cannot exceed {maxLength} characters")
                .MinimumLength(minLength).WithMessage($"{fieldName} must be at least {minLength} characters long")
                .Must(value => !SqlInjectionRegex.IsMatch(value == null ? "" : value))
                .WithMessage($"{fieldName} contains invalid characters or SQL injection patterns.")
                .Must(value => UrlRegex.IsMatch(value?.Trim() ?? "")).WithMessage($"{fieldName} is not a valid URL.");
            return rule;
        }

        // Description Validation Rule

        public static IRuleBuilderOptions<T, string?> ValidDescriptionString<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName, int minLength = 0, int maxLength = 250)
        {
            var rule = ruleBuilder
                
                .MinimumLength(minLength).WithMessage($"{fieldName} must be at least {minLength} characters long")
                .MaximumLength(maxLength).WithMessage($"{fieldName} cannot exceed {maxLength} characters");
            return rule;
        }

        // String Validation Rule
        public static IRuleBuilderOptions<T, string?> PasswordString<T>(this IRuleBuilder<T, string?> ruleBuilder, string fieldName, int minLength = 0, int maxLength = 250)
        {
            var rule = ruleBuilder
                .NotNull().WithMessage($"{fieldName} is required")
                .NotEmpty().WithMessage($"{fieldName} is required")
                .Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage($"{fieldName} is required")
                .MaximumLength(maxLength).WithMessage($"{fieldName} cannot exceed {maxLength} characters")
                .MinimumLength(minLength).WithMessage($"{fieldName} must be at least {minLength} characters long")
                .Must(value => !SqlInjectionRegex.IsMatch(value == null ? "" : value))
                .WithMessage($"{fieldName} contains invalid characters or SQL injection patterns.")
                .Must(value => PasswordRegex.IsMatch(value ?? "")).WithMessage($"{fieldName} contains invalid characters or special characters");
            return rule;
        }
    }
}
