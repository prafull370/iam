using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkDeviceReset;
using ModelLayer.Models.IAM.BulkUpload;
using RepositoryLayer.IAM.BulkDeviceReset;
using System.Data;
using System.Text.Json;

namespace DataLayer.IAM.BulkDeviceReset
{
    // constructor/fields: copy the style of your existing BulkUploadService if it differs
    internal class BulkDeviceResetService : IBulkDeviceResetService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;

        public BulkDeviceResetService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }

        public async Task<ResponseModel<string>> SaveFiles(BulkDeviceResetRequestModel objmodel)
        {
            DynamicParameters param = new();
            param.Add("p_file_name", objmodel.File_Name);
            param.Add("p_file_type", objmodel.File_Type);
            param.Add("p_file_size", objmodel.File_Size);
            param.Add("p_file_path", objmodel.File_Path);
            param.Add("p_total_count", objmodel.Total_Count);
            param.Add("p_remarks", objmodel.Remarks);

            return await FactoryService.dbmanager.Execute_SP_Insert<string>(
                "iam.sp_bulk_device_reset_upload", param, LoggedUserData);
        }

        public async Task<ResponseModel<List<BulkDeviceResetResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters param = new();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<BulkDeviceResetResponseModel>(
                "iam.sp_bulk_device_reset_get", param, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<BulkDeviceResetResponseModel>> GetById(int id)
        {
            DynamicParameters param = new();
            param.Add("p_id", id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<BulkDeviceResetResponseModel>(
                "iam.sp_bulk_device_reset_getbyid", param, LoggedUserData);
        }

        public async Task<ResponseModel<string>> SaveBulkDeviceReset(BUlkDataRequestModel<List<BulkDeviceResetExcelModel>> objmodel)
        {
            DynamicParameters param = new();
            param.Add("p_batch_id", objmodel.File_Id);
            param.Add("p_actor_id", objmodel.User);      // background call has no token, so the user id comes from the model

            var rows = (objmodel.Bulk_Data ?? new())
                .Select(x => new { login_id = x.Login_Id})
                .ToList();
            param.Add("p_json_data", JsonSerializer.Serialize(rows));

            return await FactoryService.dbmanager.Execute_SP_Insert<string>(
                "iam.sp_bulk_device_reset_process", param, LoggedUserData);
        }

        public async Task<ResponseModel<List<Dictionary<string, object>>>> GetLogsById(int id)
        {
            DynamicParameters param = new();
            param.Add("p_batch_id", id);
            var dt = await FactoryService.dbmanager.Execute_SP_Select_DataTable(
                "iam.sp_bulk_device_reset_log_get", param, LoggedUserData);

            var list = new List<Dictionary<string, object>>();
            if (dt.ResponseData?.Rows.Count > 0)
            {
                foreach (DataRow row in dt.ResponseData.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.ResponseData.Columns)
                        dict[col.ColumnName] = row[col];
                    list.Add(dict);
                }
            }
            return new ResponseModel<List<Dictionary<string, object>>>
            {
                Status = dt.Status,
                Message = dt.Message,
                ResponseType = dt.ResponseType,
                ResponseData = list
            };
        }
    }
}
