using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Department;

namespace RepositoryLayer.IAM.IDepartment
{
    public interface IDepartmentService
    {
        Task<ResponseModel<string>> Save(DepartmentRequestModel objmodel);
        Task<ResponseModel<string>> Update(DepartmentRequestModel objmodel);
        Task<ResponseModel<List<DepartmentResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<DepartmentResponseModel>> GetById(int Department_Id);
    }
}
