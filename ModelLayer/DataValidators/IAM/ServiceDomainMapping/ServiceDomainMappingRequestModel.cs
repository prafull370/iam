using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.ServiceDomainMapping;

namespace ModelLayer.DataValidators.IAM.ServiceDomainMapping
{
    public class ServiceDomainMappingRequestModelValidator : AbstractValidator<ServiceDomainMappingRequestModel>
    {
        public ServiceDomainMappingRequestModelValidator()
        {
            RuleFor(x => x.Discom_Id).ValidIntDropdown("discom");
            RuleFor(x => x.Product_Id).ValidIntDropdown("product");
            RuleFor(x => x.Product_Url).ValidUrlString("Product Url", 0, 80);
            //RuleFor(x => x.Product_Api_Url).ValidString("Product API Url", 0, 60);
            RuleFor(x => x.Product_Api_Url).ValidUrlString("Product API Url", 0, 80);
            RuleFor(x => x.Service_Data).NotNull().WithMessage("PLease select service")
              .Must(x => x.Any()).WithMessage("Please select at least one service");
            // No duplicate Service_Id values
            RuleFor(x => x.Service_Data)
                .Must(serviceList => serviceList.Select(s => s.Service_Id).Distinct().Count() == serviceList.Count)
                .WithMessage("Duplicate services are not allowed");
            RuleForEach(x => x.Service_Data).SetValidator(new ServiceDataModelValidator());
        }
    }
    public class ServiceDataModelValidator : AbstractValidator<ServiceDataModel>
    {
        public ServiceDataModelValidator()
        {
            RuleFor(x => x.Service_Id).ValidIntDropdown("service");
            //RuleFor(x => x.Service_Url).ValidString("Service Url");
            RuleFor(x => x.Service_Url).ValidUrlString("Service Url");
            //RuleFor(x => x.Service_Api_Url).ValidString("Service API Url");
            RuleFor(x => x.Service_Api_Url).ValidUrlString("Service API Url");
        }
    }
}
