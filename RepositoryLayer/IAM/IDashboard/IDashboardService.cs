using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IAM.IDashboard
{
    public interface IDashboardService
    {
        Task<ResponseModel<DashboardResponseModel>> GetKpi();
        Task<ResponseModel<List<DashboardRoleDistributionResponseModel>>> GetRoleDistribution();
        Task<ResponseModel<DashboardSecurityResponseModel>> GetSecurity();
        Task<ResponseModel<List<DashboardRecentActivityResponseModel>>> GetRecentActivity();
        Task<ResponseModel<List<DashboardLoginTrendResponseModel>>> GetLoginTrend();
    }
}
