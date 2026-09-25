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
using ModelLayer.Models.IAM.ServiceDomainMapping;
using RepositoryLayer.IAM.IServiceDomainMapping;

namespace DataLayer.IAM.ServiceDomainMapping
{
    internal class ServiceDomainMappingService:IServiceDomainMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ServiceDomainMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ServiceDomainMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
                           objmodel.Service_Data.Select(x => new
                           {
                               service_id = x.Service_Id,
                               service_url = x.Service_Url,
                               service_api_url = x.Service_Api_Url
                           }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_discom_id", objmodel.Discom_Id);
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_product_url", objmodel.Product_Url);
            dynParameters.Add("p_product_api_url", objmodel.Product_Api_Url);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_discom_product_service_domain_mapping_save", dynParameters, LoggedUserData);
        }


        public async Task<ResponseModel<List<ServiceDomainMappingResponseModel>>> Get(ServiceDomainGetModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_discom_id", objmodel.Discom_Id);
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<ServiceDomainMappingResponseModel>("iam.sp_discom_product_service_domain_mapping_get", dynParameters, LoggedUserData);
        }
    }
}
