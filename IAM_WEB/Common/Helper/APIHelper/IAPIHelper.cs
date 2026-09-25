using ModelLayer.Models.Common;

namespace IAM_WEB.Common.Helper.APIHelper
{
    public interface IAPIHelper
    {
        Task<ResponseModel<T>> GetAsync<T>(string ApiUrl, string? bearerToken = null);
        Task<ResponseModel<T>> PostAsync<T>(string ApiUrl, object data, string? bearerToken = null);
        Task<ResponseModel<T>> PutAsync<T>(string ApiUrl, object data, string? bearerToken = null);
        Task<ResponseModel<T>> DeleteAsync<T>(string ApiUrl, string? bearerToken = null);
    }
}
