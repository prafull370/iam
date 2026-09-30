using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using FluentValidation;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Dashboard;
using RepositoryLayer.IAM.IDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.IAM.Dashboard
{
    internal class DashboardService : IDashboardService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;

        public DashboardService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }


        #region Top 5 KPIs

        public async Task<ResponseModel<DashboardResponseModel>> GetKpi()
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_Single<DashboardResponseModel>("iam.sp_dashboard_kpi_get", dynParameters, LoggedUserData);
        }

        #endregion

        #region Role Distribution

        public async Task<ResponseModel<List<DashboardRoleDistributionResponseModel>>> GetRoleDistribution()
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List<DashboardRoleDistributionResponseModel>("iam.sp_dashboard_role_distribution_get", dynParameters, LoggedUserData);
        }

        #endregion

        #region Security Overview

        public async Task<ResponseModel<DashboardSecurityResponseModel>> GetSecurity()
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_Single<DashboardSecurityResponseModel>("iam.sp_dashboard_security_get", dynParameters, LoggedUserData);
        }

        #endregion

        #region Recent Activity
        public async Task<ResponseModel<List<DashboardRecentActivityResponseModel>>> GetRecentActivity()
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List<DashboardRecentActivityResponseModel>("iam.sp_dashboard_recent_activity_get", dynParameters, LoggedUserData);
        }

        #endregion

        #region Login Trend (last 7 days)

        public async Task<ResponseModel<List<DashboardLoginTrendResponseModel>>> GetLoginTrend()
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List<DashboardLoginTrendResponseModel>("iam.sp_dashboard_login_trend_get", dynParameters, LoggedUserData);
        }
        #endregion

    }
}
