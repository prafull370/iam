using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.DataSyncing;
using ModelLayer.Models.IAM.Department;

namespace ModelLayer.DataValidators.IAM.DataSyncing
{
    public class DataSyncRequestModelValidator : AbstractValidator<DataSyncRequestModel>
    {
        public DataSyncRequestModelValidator()
        {
            RuleFor(x => x.Sync_Type_Id).ValidIntDropdown("Type ");
            RuleFor(x => x.Product_Id).ValidIntDropdown("Product ");
        }
    }
}