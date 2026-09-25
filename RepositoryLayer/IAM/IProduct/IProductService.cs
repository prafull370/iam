using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Product;

namespace RepositoryLayer.IAM.IProduct
{
    public interface IProductService
    {
        Task<ResponseModel<string>> Save(ProductRequestModel objmodel);
        Task<ResponseModel<string>> Update(ProductRequestModel objmodel);
        Task<ResponseModel<List<ProductResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<ProductResponseModel>> GetById(int Product_Id);
    }
}
