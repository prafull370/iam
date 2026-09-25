using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.RolePageAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.IAM.IRolePageActionService
{
    public interface  IRolePageActionService
    {
        Task<ResponseModel<string>> Save(RolePageActionRequestModel objmodel);
        Task<ResponseModel<RolePageActionResponseModel>> Get(int Product_Id);
        //Task<ResponseModel<Dictionary<string, object>>> DownloadList(int Product_ID);
    }
}
