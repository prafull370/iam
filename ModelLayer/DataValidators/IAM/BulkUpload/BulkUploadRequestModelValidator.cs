using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DataValidaters.Common;
using ModelLayer.Models.IAM.BulkUpload;
using ModelLayer.Models.IAM.Department;

namespace ModelLayer.DataValidators.IAM.BulkUpload
{
    public class BulkUploadRequestModelValidator : AbstractValidator<BulkUploadFilesRequestModel>
    {
        public BulkUploadRequestModelValidator()
        {
            RuleFor(x => x.Bulk_Type_Id).ValidIntDropdown("Upload Type ");
        }
    }
}
