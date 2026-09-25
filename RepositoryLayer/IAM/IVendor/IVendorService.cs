using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Vendor;

namespace RepositoryLayer.IAM.IVendor
{
    public interface IVendorService
    {
        Task<ResponseModel<string>> Save(VendorRequestModel objmodel);
        Task<ResponseModel<string>> Update(VendorRequestModel objmodel);
        Task<ResponseModel<List<VendorResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<VendorResponseModel>> GetById(int Vendor_Id);
    }
}
