using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DataSyncing;
using ModelLayer.Models.IAM.Department;
using RepositoryLayer.IAM.IDataSyncing;

namespace DataLayer.IAM.DataSyncing
{
    internal class DataSyncService:IDataSyncService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DataSyncService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<List<DataSyncResponseModel>>> Get(DataSyncRequestModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_sync_type_id", pagination.Sync_Type_Id);
            dynParameters.Add("p_product_id", pagination.Product_Id);
            dynParameters.Add("p_sync_status", pagination.Status);
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<DataSyncResponseModel>("iam.sp_syncing_data_get", dynParameters, pagination, LoggedUserData);
        }
        public async Task<ResponseModel<List<T>>> GetUser_Data<T>(int userId, int syncTypeId)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_sync_type_id", syncTypeId);
            dynParameters.Add("p_id", userId);

            return await FactoryService.dbmanager
                .Execute_SP_Select_List<T>(
                    "iam.sp_syncing_data_get_by_sync_type",
                    dynParameters,
                    LoggedUserData
                );
        }

        public async Task<ResponseModel<List<ProductDataSyncModel>>> Get_Product_Data(int Product_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", Product_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<ProductDataSyncModel>("iam.sp_product_info_for_sync_data_get", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> SaveDataSyncLog(DataSyncLogModel model)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_input_record_id", model.Record_Id);
            dynParameters.Add("p_sync_type_id", model.Sync_Type_Id);
            dynParameters.Add("p_product_id", model.Product_Id);
            dynParameters.Add("p_sync_status", model.Sync_Status);
            dynParameters.Add("p_input_message", model.Message);
            
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_sync_data_log_save", dynParameters, LoggedUserData);
        }
    }
}
