using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DataSyncing;

namespace RepositoryLayer.IAM.IDataSyncing
{
    public interface IDataSyncService
    {
        Task<ResponseModel<List<DataSyncResponseModel>>> Get(DataSyncRequestModel pagination);
        Task<ResponseModel<List<T>>> GetUser_Data<T>(int userId, int syncTypeId);
        Task<ResponseModel<string>> SaveDataSyncLog(DataSyncLogModel model);
        Task<ResponseModel<List<ProductDataSyncModel>>> Get_Product_Data(int Product_Id);
    }
}
