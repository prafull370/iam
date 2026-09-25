using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Discom;
using ModelLayer.Models.IAM.DiscomProductMapping;
using NpgsqlTypes;
using RepositoryLayer.IAM.IDiscomProductMapping;

namespace DataLayer.IAM.DiscomProductMapping
{
    internal class DiscomProductMappingService: IDiscomProductMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DiscomProductMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(DiscomProductMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
            objmodel.Product_Ids.Select(pid => new { product_id = pid }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_discom_id", objmodel.Discom_Id);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_discom_product_mapping_save", dynParameters, LoggedUserData);
        }
        

        public async Task<ResponseModel<List<DiscomProductMappingResponseModel>>> Get(int Discom_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_discom_id", Discom_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<DiscomProductMappingResponseModel>("iam.sp_discom_product_mapping_get", dynParameters, LoggedUserData);
        }
    }
}
