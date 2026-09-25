using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Company;

namespace RepositoryLayer.IAM.ICompany
{
    public interface ICompanyService
    {
        Task<ResponseModel<string>> Save(CompanyRequestModel objmodel);
        Task<ResponseModel<string>> Update(CompanyRequestModel objmodel);
        Task<ResponseModel<List<CompanyResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<CompanyResponseModel>> GetById(int Company_Id);
    }
}
