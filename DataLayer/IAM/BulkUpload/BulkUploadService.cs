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
using DocumentFormat.OpenXml.Spreadsheet;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkUpload;
using ModelLayer.Models.IAM.Department;
using RepositoryLayer.IAM.BulkUpload;

namespace DataLayer.IAM.BulkUpload
{
    internal class BulkUploadService:IBulkUploadService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public BulkUploadService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> SaveFiles(BulkUploadFilesRequestModel objModel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_upload_type_id", objModel.Bulk_Type_Id);
            dynParameters.Add("p_file_type", objModel.File_Type);
            dynParameters.Add("p_file_name", objModel.File_Name);
            dynParameters.Add("p_file_path", objModel.File_Path);
            dynParameters.Add("p_file_size", objModel.File_Size);
            dynParameters.Add("p_total_count", objModel.Total_Count);
            // Adding all the parameters for the stored procedure


            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_bulk_upload_files_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<List<BulkUploadFilesResponseModel>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<BulkUploadFilesResponseModel>("iam.sp_bulk_upload_files_get", dynParameters, pagination, LoggedUserData);
        }
        public async Task<ResponseModel<BulkUploadFilesResponseModel>> GetById(int Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_file_id", Id);
            return await FactoryService.dbmanager.Execute_SP_Select_Single<BulkUploadFilesResponseModel>("iam.sp_bulk_upload_files_get_by_id", dynParameters,LoggedUserData);
        }
        public async Task<ResponseModel<List<Dictionary<string, object>>>> GetLogsById(int Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_file_id", Id);
            var datatableResponseData= await FactoryService.dbmanager.Execute_SP_Select_DataTable("iam.sp_bulk_upload_log_get_by_id", dynParameters, LoggedUserData);
            var table = datatableResponseData.ResponseData;
            var list = new List<Dictionary<string, object>>();
            if (table?.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    list.Add(dict);
                }
            }
            return new ResponseModel<List<Dictionary<string, object>>>
            {
                Status = datatableResponseData.Status,
                Message = datatableResponseData.Message,
                ResponseType = datatableResponseData.ResponseType,
                ResponseData = list
            };
        }
        public async Task<ResponseModel<string>> SaveBulkUsers(BUlkDataRequestModel<List<BulkUserUploadModel>> objModel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_user", objModel.User);
            dynParameters.Add("p_file_id", objModel.File_Id);
            dynParameters.Add("p_json_data", JsonSerializer.Serialize(objModel.Bulk_Data));
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_bulk_user_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> SaveBulkVendors(BUlkDataRequestModel<List<BulkVendorMasterModel>> objModel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_user", objModel.User);
            dynParameters.Add("p_file_id", objModel.File_Id);
            dynParameters.Add("p_json_data", JsonSerializer.Serialize(objModel.Bulk_Data));
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_bulk_vendor_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> SaveBulkLevels(BUlkDataRequestModel<List<BulkLevelMasterModel>> objModel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_user", objModel.User);
            dynParameters.Add("p_file_id", objModel.File_Id);
            dynParameters.Add("p_json_data", JsonSerializer.Serialize(objModel.Bulk_Data));
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_bulk_level_save", dynParameters, LoggedUserData);
        }
    }
}
