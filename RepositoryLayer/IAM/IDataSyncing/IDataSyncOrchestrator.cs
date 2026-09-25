using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.IAM.DataSyncing;

namespace RepositoryLayer.IAM.IDataSyncing
{
    public interface IDataSyncOrchestrator
    {
        Task ProcessDataSyncing(
            List<DataSyncResponseModel> model,
            int syncTypeId,
            int productId);
    }

}
