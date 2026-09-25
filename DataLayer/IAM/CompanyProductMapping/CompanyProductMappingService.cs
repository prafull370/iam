using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.CompanyProductMapping;
using RepositoryLayer.IAM.ICompanyProductMapping;

namespace DataLayer.IAM.CompanyProductMapping
{
    internal class CompanyProductMappingService:ICompanyProductMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public CompanyProductMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(CompanyProductMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
            objmodel.Product_Ids.Select(pid => new { product_id = pid }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_company_id", objmodel.Company_Id);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_company_product_mapping_save", dynParameters, LoggedUserData);
        }


        public async Task<ResponseModel<List<CompanyProductMappingResponseModel>>> Get(int Company_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_company_id", Company_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<CompanyProductMappingResponseModel>("iam.sp_company_product_mapping_get", dynParameters, LoggedUserData);
        }
    }
}
