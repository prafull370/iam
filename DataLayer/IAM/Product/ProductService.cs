using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Product;
using RepositoryLayer.IAM.IProduct;

namespace DataLayer.IAM.Product
{
    internal class ProductService:IProductService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ProductService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ProductRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_name", objmodel.Product_Name);
            dynParameters.Add("p_product_short_name", objmodel.Product_Short_Name);
            dynParameters.Add("p_product_logo", objmodel.Product_Logo);
            dynParameters.Add("p_product_desc", objmodel.Product_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_version_name", objmodel.Version_Name);

            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_product_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(ProductRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_product_name", objmodel.Product_Name);
            dynParameters.Add("p_product_short_name", objmodel.Product_Short_Name);
            dynParameters.Add("p_product_logo", objmodel.Product_Logo);
            dynParameters.Add("p_product_desc", objmodel.Product_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_version_name", objmodel.Version_Name);

            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_product_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<ProductResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<ProductResponseModel>("iam.sp_product_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<ProductResponseModel>> GetById(int Product_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", Product_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<ProductResponseModel>("iam.sp_product_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
