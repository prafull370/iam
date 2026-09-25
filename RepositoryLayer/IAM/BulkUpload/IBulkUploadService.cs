using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkUpload;

namespace RepositoryLayer.IAM.BulkUpload
{
    public interface IBulkUploadService
    {
        Task<ResponseModel<string>> SaveFiles(BulkUploadFilesRequestModel objModel);
        Task<ResponseModel<BulkUploadFilesResponseModel>> GetById(int Id);
        Task<ResponseModel<List<BulkUploadFilesResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<string>> SaveBulkUsers(BUlkDataRequestModel<List<BulkUserUploadModel>> objModel);
        Task<ResponseModel<string>> SaveBulkVendors(BUlkDataRequestModel<List<BulkVendorMasterModel>> objModel);
        Task<ResponseModel<string>> SaveBulkLevels(BUlkDataRequestModel<List<BulkLevelMasterModel>> objModel);
        Task<ResponseModel<List<Dictionary<string, object>>>> GetLogsById(int Id);
    }
}
