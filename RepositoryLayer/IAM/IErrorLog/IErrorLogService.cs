using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.ErrorLog;

namespace RepositoryLayer.IAM.IErrorLog
{
    public interface IErrorLogService
    {
        //Task<ResponseModel<List<T>>> GetList<T>(DBErrorLogRequestModel model);
        Task<ResponseModel<List<DBErrorLogResponseModel>>> GetList(DBErrorLogRequestModel pagination);
    }
}
