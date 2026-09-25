using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using IAM_MODEL.DeviceReset;
using ModelLayer.Models.Common;
using RepositoryLayer.IAM.IDeviceReset;

namespace DataLayer.IAM.DeviceReset
{
    internal class DeviceReset : IDeviceResetService
    {
        #region Declaration
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DeviceReset(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        #endregion

        #region User Device List
        public async Task<ResponseModel<List<ResetDeviceIdFilter>>> Get(PaginationModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<ResetDeviceIdFilter>("iam.sp_device_get", dynParameters, pagination, LoggedUserData);
        }
        #endregion

        #region User  Device Id Update
        public async Task<ResponseModel<string>> Update(UpdateDeviceId objmodel)
        {
            try
            {
                DynamicParameters dynParameters = new DynamicParameters();
                dynParameters.Add("p_login_id", objmodel.Login_Id);
                dynParameters.Add("p_updated_by", objmodel.Updated_By);
                dynParameters.Add("p_ip_address", objmodel.Ip_Address);
                return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_reset_device_id", dynParameters, LoggedUserData);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use your logging mechanism here)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new ResponseModel<string>
                {
                    Status = false,
                    Message = "An error occurred while updating the device ID.",
                    ResponseData = null
                };
            }
        }
         #endregion
    }
}
