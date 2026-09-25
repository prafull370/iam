
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.Service;

namespace ModelLayer.DataValidators.IAM.Service
{
    public class ServiceRequestModelValidator : AbstractValidator<ServiceRequestModel>
    {
        public ServiceRequestModelValidator()
        {
            RuleFor(x => x.Service_Name).ValidString("Service Name ", 0, 30);
            RuleFor(x => x.Service_Desc).ValidStringNotRequired("Description ", 0, 200);
            RuleFor(x => x.Is_Active).ValidBoolean("IsActive");
            RuleFor(x => x.Is_Microservice).ValidBoolean("IsMicroservice");
            RuleFor(x => x.Service_Id).ValiInt("Service id").When(x => x.Service_Id != 0); // Apply only when ReportId is provided
            RuleFor(x => x.Display_Order).GreaterThan(0).WithMessage("Display Order must be greater than 0");
        }
    }
}
