using System.Data;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.User;
using Newtonsoft.Json;

namespace IAM_WEB.Common.Helper.APIHelper
{
    internal class APIHelper : IAPIHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private static IHttpContextAccessor? _httpContextAccessor;
        private IMemoryCache _cache;
        private int ExpireTime;

        public APIHelper(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor,IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _apiUrl = configuration.GetValue<string>("API:BaseUrl") ?? throw new InvalidOperationException("API base url not found.");
            _cache = cache;
            ExpireTime = configuration.GetValue<int>("SSO:ExpireTime")
;        }

        public async Task<ResponseModel<T>> GetAsync<T>(string ApiUrl, string? bearerToken = null)
        {
           await AddAuthorizationHeader(bearerToken);
            var response = await _httpClient.GetAsync(_apiUrl + ApiUrl);
            return await HandleResponse<T>(response);
        }

        public async Task<ResponseModel<T>> PostAsync<T>(string ApiUrl, object data, string? bearerToken = null)
        {
           await AddAuthorizationHeader(bearerToken);
            var content = GetJsonContent(data);
            var response = await _httpClient.PostAsync(_apiUrl + ApiUrl, content);
            return await HandleResponse<T>(response);
        }

        public async Task<ResponseModel<T>> PutAsync<T>(string ApiUrl, object data, string? bearerToken = null)
        {
           await AddAuthorizationHeader(bearerToken);
            var content = GetJsonContent(data);
            var response = await _httpClient.PutAsync(_apiUrl + ApiUrl, content);
            return await HandleResponse<T>(response);
        }

        public async Task<ResponseModel<T>> DeleteAsync<T>(string ApiUrl, string? bearerToken = null)
        {
           await AddAuthorizationHeader(bearerToken);
            var response = await _httpClient.DeleteAsync(_apiUrl + ApiUrl);
            return await HandleResponse<T>(response);
        }

        // ✅ Automatically adds token from cookie if not explicitly provided
        private async Task<bool> AddAuthorizationHeader(string? bearerToken)
        {
            var context = _httpContextAccessor?.HttpContext;

            // Step 1️⃣: Try to get token from parameter
            if (string.IsNullOrEmpty(bearerToken))
            {
                string Token_Id = context?.Session.GetString("Token_Id") ?? "";
                if (string.IsNullOrWhiteSpace(Token_Id))
                {
                    context?.Request.Cookies.TryGetValue("IAM_Token", out Token_Id);
                    if (!string.IsNullOrWhiteSpace(Token_Id))
                    {
                        context?.Session.SetString("Token_Id", Token_Id);
                        bearerToken = _cache.Get<string>("API_Token_" + Token_Id);
                    }
                }
                else
                {
                    // Retrieve from cache if Token_Id exists
                    bearerToken = _cache.Get<string>("API_Token_" + Token_Id);
                }

                // Step 3️⃣: If still null, try from cookie
                if (string.IsNullOrEmpty(bearerToken) && !string.IsNullOrWhiteSpace(Token_Id))
                {
                    var queryParams = new Dictionary<string, string>
                    {
                        ["iamtoken"] = Token_Id,
                    };
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                    string url = QueryHelpers.AddQueryString("Login/getbyiamtoken", queryParams);
                    var response = await _httpClient.GetAsync(_apiUrl + url);
                    var Data = await HandleResponse<LoginResponseModel>(response);
                    bearerToken = Data?.ResponseData?.API_Token;

                    // ✅ Only cache if we have both Token_Id and API_Token
                    if (!string.IsNullOrWhiteSpace(Token_Id) && !string.IsNullOrWhiteSpace(bearerToken))
                    {
                        _cache.Set("API_Token_" + Token_Id, bearerToken, TimeSpan.FromHours(ExpireTime));
                    }
                }
            }

            // Step 5️⃣: Set Authorization header
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            // Step 6️⃣: Forward the WEB checkpoint's audit decision to the API
            _httpClient.DefaultRequestHeaders.Remove("X-Audit-Enabled");
            var items = context?.Items;
            if (items != null && items.TryGetValue("Is_Activity_log", out var auditFlag) && auditFlag is bool auditEnabled)
            {
                _httpClient.DefaultRequestHeaders.Add("X-Audit-Enabled", auditEnabled ? "true" : "false");
            }
            return true;
        }
    

        private static StringContent GetJsonContent(object data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }

        private static async Task<ResponseModel<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var context = _httpContextAccessor?.HttpContext;

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // ✅ FIX: Don't redirect here - return error response
                return new ResponseModel<T>
                {
                    Status = false,
                    Message = "Session expired or invalid token",
                    ResponseType = "UNAUTHORIZED",
                    StatusCode = "401"
                };
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new ResponseModel<T>
                {
                    Status = false,
                    Message = "Access forbidden",
                    ResponseType = "FORBIDDEN",
                    StatusCode = "403"
                };
            }

            // ✅ Convert DataTable response if needed
            if (typeof(T) == typeof(DataTable))
            {
                var tempResponse = JsonConvert.DeserializeObject<ResponseModel<List<Dictionary<string, object>>>>(json);

                var dt = new DataTable();
                if (tempResponse!.ResponseData!.Count > 0)
                {
                    foreach (var col in tempResponse.ResponseData[0].Keys)
                        dt.Columns.Add(col);

                    foreach (var row in tempResponse.ResponseData)
                    {
                        var dr = dt.NewRow();
                        foreach (var key in row.Keys)
                            dr[key] = row[key] is null || row[key].ToString() == "{}" ? DBNull.Value : row[key];
                        dt.Rows.Add(dr);
                    }
                }

                var finalResponse = new ResponseModel<DataTable>
                {
                    Status = tempResponse.Status,
                    Message = tempResponse.Message,
                    ResponseType = tempResponse.ResponseType,
                    ResponseData = dt
                };

                return (ResponseModel<T>)(object)finalResponse!;
            }

            return JsonConvert.DeserializeObject<ResponseModel<T>>(json)!;
        }
    }
}
