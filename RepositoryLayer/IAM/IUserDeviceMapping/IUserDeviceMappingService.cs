using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.UserDeviceMapping;

namespace RepositoryLayer.IAM.IUserDeviceMapping
{
    public interface IUserDeviceMappingService
    {
        Task<ResponseModel<List<UserDeviceMappingResponseModel>>> Get(int User_Id);
        Task<ResponseModel<string>> Save(UserDeviceMappingRequestModel objmodel);
    }
}
