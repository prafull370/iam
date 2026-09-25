using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Service;
using RepositoryLayer.IAM.IService;

namespace DataLayer.IAM.Service
{
    internal class ServicesService:IServicesService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public ServicesService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(ServiceRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_service_name", objmodel.Service_Name);
            dynParameters.Add("p_service_desc", objmodel.Service_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_is_microservice", objmodel.Is_Microservice);
            dynParameters.Add("p_service_logo", objmodel.Service_Logo);
            dynParameters.Add("p_display_order", objmodel.Display_Order);

            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_service_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(ServiceRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_service_id", objmodel.Service_Id);
            dynParameters.Add("p_service_name", objmodel.Service_Name);
            dynParameters.Add("p_service_desc", objmodel.Service_Desc);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_is_microservice", objmodel.Is_Microservice);
            dynParameters.Add("p_service_logo", objmodel.Service_Logo);
            dynParameters.Add("p_display_order", objmodel.Display_Order);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_service_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<ServiceResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<ServiceResponseModel>("iam.sp_service_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<ServiceResponseModel>> GetById(int Service_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_service_id", Service_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<ServiceResponseModel>("iam.sp_service_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
