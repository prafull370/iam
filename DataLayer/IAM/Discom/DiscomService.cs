using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Discom;
using RepositoryLayer.IAM.IDiscom;

namespace DataLayer.IAM.Discom
{
    internal class DiscomService:IDiscomService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DiscomService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(DiscomRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_discom_name", objmodel.Discom_Name);
            dynParameters.Add("p_discom_name_regional", objmodel.Discom_Name_Regional);
            dynParameters.Add("p_discom_short_code", objmodel.Discom_Short_Code);
            dynParameters.Add("p_discom_logo", objmodel.Discom_Logo);
            dynParameters.Add("p_remarks", objmodel.Remarks);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_discom_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(DiscomRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_discom_id", objmodel.Discom_Id);
            dynParameters.Add("p_discom_name", objmodel.Discom_Name);
            dynParameters.Add("p_discom_name_regional", objmodel.Discom_Name_Regional);
            dynParameters.Add("p_discom_short_code", objmodel.Discom_Short_Code);
            dynParameters.Add("p_discom_logo", objmodel.Discom_Logo);
            dynParameters.Add("p_remarks", objmodel.Remarks);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_discom_update", dynParameters, LoggedUserData);

        }

        public async Task<ResponseModel<List<DiscomResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<DiscomResponseModel>("iam.sp_discom_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<DiscomResponseModel>> GetById(int Discom_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_discom_id", Discom_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<DiscomResponseModel>("iam.sp_discom_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
