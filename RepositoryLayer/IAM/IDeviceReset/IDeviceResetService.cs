
using IAM_MODEL.DeviceReset;
using ModelLayer.Models.Common;

namespace RepositoryLayer.IAM.IDeviceReset
{
    public interface IDeviceResetService
    {
        Task<ResponseModel<List<ResetDeviceIdFilter>>> Get(PaginationModel pagination);
        Task<ResponseModel<string>> Update(UpdateDeviceId objmodel);
    }
}
