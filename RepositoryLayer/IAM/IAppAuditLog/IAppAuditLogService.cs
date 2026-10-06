using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.AppAuditLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IAM.IAppAuditLog
{
    public interface IAppAuditLogService
    {
        Task<ResponseModel<List<AppAuditUserResponseModel>>> GetUsers(AppAuditLogRequestModel model);
        Task<ResponseModel<List<AppAuditDetailResponseModel>>> GetDetail(string loginId);
        Task<ResponseModel<AppAuditStatsResponseModel>> GetStats();
    }
}
