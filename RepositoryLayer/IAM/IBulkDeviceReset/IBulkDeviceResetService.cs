using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.BulkDeviceReset;
using ModelLayer.Models.IAM.BulkUpload;
using System.Data;

namespace RepositoryLayer.IAM.BulkDeviceReset
{
    public interface IBulkDeviceResetService
    {
        Task<ResponseModel<string>> SaveFiles(BulkDeviceResetRequestModel objmodel);
        Task<ResponseModel<List<BulkDeviceResetResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<BulkDeviceResetResponseModel>> GetById(int id);
        Task<ResponseModel<string>> SaveBulkDeviceReset(BUlkDataRequestModel<List<BulkDeviceResetExcelModel>> objmodel);
        //Task<ResponseModel<DataTable>> GetLogsById(int id);
        Task<ResponseModel<List<Dictionary<string, object>>>> GetLogsById(int id);

    }
}
