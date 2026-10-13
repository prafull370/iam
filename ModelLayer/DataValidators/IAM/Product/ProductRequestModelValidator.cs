using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Product;

namespace ModelLayer.DataValidators.IAM.Product
{
    public class ProductRequestModelValidator : AbstractValidator<ProductRequestModel>
    {
        public ProductRequestModelValidator()
        {
            RuleFor(x => x.Product_Name).ValidString("Product Name ", 0, 20);
            RuleFor(x => x.Product_Short_Name).ValidString("Product Short Name ", 0, 10);
            RuleFor(x => x.Product_Desc).ValidStringNotRequired("Description ", 0, 200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Product_Id).ValiInt("Product id").When(x => x.Product_Id != 0); // Apply only when ReportId is provided
            RuleFor(x => x.Version_Name).ValidString("Version ", 0, 20);
        }
    }
}
