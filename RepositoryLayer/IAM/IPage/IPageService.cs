using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Page;

namespace RepositoryLayer.IAM.IPage
{
    public interface IPageService
    {
        Task<ResponseModel<string>> Save(PageRequestModel objmodel);
        Task<ResponseModel<string>> Update(PageRequestModel objmodel);
        Task<ResponseModel<List<PageResponseModel>>> Get(PageGetRequestModel pagination);
        Task<ResponseModel<PageResponseModel>> GetById(int Page_Id);
    }
}
