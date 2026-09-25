using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Department;
using ModelLayer.Models.IAM.RolePageMapping;
using ModelLayer.Models.IAM.UserDeviceMapping;
using RepositoryLayer.IAM.IUserDeviceMapping;

namespace DataLayer.IAM.UserDeviceMapping
{
    internal class UserDeviceMappingService:IUserDeviceMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public UserDeviceMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(UserDeviceMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
                           objmodel.DeviceMappings.Select(x => new
                           {
                               applicationId = x.ApplicationId,
                               deviceId = x.DeviceId
                           }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_user", objmodel.User_Id);
            dynParameters.Add("p_mapping_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_user_app_device_mapping_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<List<UserDeviceMappingResponseModel>>> Get(int User_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_user", User_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<UserDeviceMappingResponseModel>("iam.sp_app_user_device_get", dynParameters, LoggedUserData);
        }
    }
}
