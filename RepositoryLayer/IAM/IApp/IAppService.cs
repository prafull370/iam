using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.App;

namespace RepositoryLayer.IAM.IApp
{
    public interface IAppService
    {
        Task<ResponseModel<string>> Save(AppRequestModel objmodel);
        Task<ResponseModel<string>> Update(AppRequestModel objmodel);
        Task<ResponseModel<List<AppResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<AppResponseModel>> GetById(int App_Id);
    }
}
