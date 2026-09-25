using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Configuration;

namespace RepositoryLayer.IAM.IConfiguration
{
    public interface IConfigurationService
    {
        Task<ResponseModel<string>> Save(ConfigurationRequestModel objmodel);
        Task<ResponseModel<string>> Update(ConfigurationRequestModel objmodel);
        Task<ResponseModel<List<ConfigurationResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<ConfigurationResponseModel>> GetById(int Config_Id);
    }
}
