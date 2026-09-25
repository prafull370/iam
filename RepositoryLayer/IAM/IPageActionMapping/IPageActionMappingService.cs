using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.PageActionMapping;

namespace RepositoryLayer.IAM.IPageActionMapping
{
    public interface IPageActionMappingService
    {
        Task<ResponseModel<PageActionMappingResponseModel>> GetMapping(int Page_Id);
        Task<ResponseModel<string>> SaveMapping(PageActionMappingRequestModel objmodel);
       }
}
