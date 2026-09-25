using System.Text.Json;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ProductServiceMapping;
using RepositoryLayer.IAM.IProductServiceMapping;

namespace DataLayer.IAM.ProductServiceMapping
{
    internal class ProductServiceMappingService:IProductServiceMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ProductServiceMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ProductServiceMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
            objmodel.Service_Ids.Select(pid => new { service_id = pid }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_product_service_mapping_save", dynParameters, LoggedUserData);
        }


        public async Task<ResponseModel<List<ProductServiceMappingResponseModel>>> Get(int Product_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", Product_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<ProductServiceMappingResponseModel>("iam.sp_product_service_mapping_get", dynParameters, LoggedUserData);
        }
    }
}
