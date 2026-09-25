using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ProductServiceMapping;

namespace RepositoryLayer.IAM.IProductServiceMapping
{
    public interface IProductServiceMappingService
    {
        Task<ResponseModel<string>> Save(ProductServiceMappingRequestModel objmodel);
        Task<ResponseModel<List<ProductServiceMappingResponseModel>>> Get(int Product_Id);
    }
}
