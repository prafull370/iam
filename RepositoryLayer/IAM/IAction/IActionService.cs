using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Action;

namespace RepositoryLayer.IAM.IAction
{
    public interface IActionService
    {
        Task<ResponseModel<string>> Save(ActionRequestModel objmodel);
        Task<ResponseModel<string>> Update(ActionRequestModel objmodel);
        Task<ResponseModel<List<ActionResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<ActionResponseModel>> GetById(int Action_Id);
    }
}
