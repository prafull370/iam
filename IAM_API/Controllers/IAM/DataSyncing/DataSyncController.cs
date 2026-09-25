using System.Text.Json;
using IAM_API.Common.Helper.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DataSyncing;

namespace IAM_API.Controllers.IAM.DataSyncing
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DataSyncController : ControllerBase
    {
        private readonly RepositoryDependencyService _repository;
        public DataSyncController(RepositoryDependencyService service)
        {
            _repository = service;
        }
        [HttpPost("get")]
        public async Task<IActionResult> get([FromBody] DataSyncRequestModel model)
        {
            var validation = await ValidationHelper.ValidateModel<DataSyncRequestModel>(HttpContext.RequestServices, model);
            if (!validation.Status)
            {
                return StatusCode(validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, validation);
            }
            var result = await _repository.datasyncservice.Get(model);
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        [HttpPost("syncdata")]
        public async Task<IActionResult> syncdata([FromBody] DataSyncRequestModel model)
        {
            var validation = await ValidationHelper.ValidateModel<DataSyncRequestModel>(HttpContext.RequestServices, model);
            if (!validation.Status)
            {
                return StatusCode(validation.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, validation);
            }
            var result = await _repository.datasyncservice.Get(model);
            if (result.ResponseData == null || !result.ResponseData.Any())
            {
                result.Message = "There is not data available for syncing.";
            }
            else
            {
                result.Message = "Data syncing start in backgroud.";
                // ✅ 2. Run Background Sync Task (Fire & Forget)
                _ = Task.Run(async () =>
                {
                    await ProcessDataSyncing(result.ResponseData, model.Sync_Type_Id, model.Product_Id);  // ✅ Background execution
                });
            }
            return StatusCode(result.Status ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, result);

        }
        private async Task ProcessDataSyncing(List<DataSyncResponseModel> model, int Sync_Type_Id, int Product_Id)
        {
            var product_data = await _repository.datasyncservice.Get_Product_Data(Product_Id);

            if (product_data != null && product_data.ResponseData != null && product_data.ResponseData.Any())
            {
                foreach (var product in product_data.ResponseData)
                {
                    string Product_Api_Url = "" + product.Product_API_Url;
                    foreach (var item in model)
                    {
                        if (Sync_Type_Id == 1)
                        {

                            var userdata = await _repository.datasyncservice.GetUser_Data<UserSyncModel>(item.Record_Id, Sync_Type_Id);
                            if (userdata != null && userdata.ResponseData != null && userdata.ResponseData.Any())
                            {

                                foreach (UserSyncModel data in userdata.ResponseData)
                                {
                                    var usersyncresponse = await PostAsync<string>(Product_Api_Url + "DataSync/SyncUser", data);
                                    DataSyncLogModel logmodel = new DataSyncLogModel();
                                    logmodel.Record_Id = item.Record_Id;
                                    logmodel.Sync_Type_Id = Sync_Type_Id;
                                    logmodel.Sync_Status = usersyncresponse?.Status ?? false;
                                    logmodel.Message = usersyncresponse?.Message ?? "No response received from SyncUser API.";
                                    logmodel.Product_Id = Product_Id;
                                    var LogResponse = await _repository.datasyncservice.SaveDataSyncLog(logmodel);
                                }

                            }
                        }
                        else if (Sync_Type_Id == 2)
                        {

                            var hierarchydata = await _repository.datasyncservice.GetUser_Data<HierarchyResponseModel>(item.Record_Id, Sync_Type_Id);
                            if (hierarchydata != null && hierarchydata.ResponseData != null && hierarchydata.ResponseData.Any())
                            {
                                UserHierarchySyncModel UserhierarchyData = new UserHierarchySyncModel();
                                UserhierarchyData.User_Id = hierarchydata.ResponseData[0].User_Id;
                                UserhierarchyData.Created_By = hierarchydata.ResponseData[0].Created_By;
                                UserhierarchyData.Last_Updated_By = hierarchydata.ResponseData[0].Last_Updated_By;
                                UserhierarchyData.Ip_Address = hierarchydata.ResponseData[0].Ip_Address;
                                UserhierarchyData.Hierarchy_List = new List<HierarchySyncModel>(); // ✅ REQUIRED
                                foreach (HierarchyResponseModel data in hierarchydata.ResponseData)
                                {
                                    UserhierarchyData.Hierarchy_List.Add(new HierarchySyncModel
                                    {
                                        Company_Id = data.Company_Id,
                                        Discom_Id = data.Discom_Id,
                                        Level1_Id = data.Level1_Id,
                                        Level2_Id = data.Level2_Id,
                                        Level3_Id = data.Level3_Id,
                                        Level4_Id = data.Level4_Id,
                                        Level5_Id = data.Level5_Id,
                                        Level6_Id = data.Level6_Id

                                    });
                                }
                                var usersyncresponse = await PostAsync<string>(Product_Api_Url + "DataSync/SyncUserHierarchy", UserhierarchyData);
                                DataSyncLogModel logmodel = new DataSyncLogModel();
                                logmodel.Record_Id = item.Record_Id;
                                logmodel.Sync_Type_Id = Sync_Type_Id;
                                logmodel.Sync_Status = usersyncresponse.Status;
                                logmodel.Message = usersyncresponse.Message;
                                logmodel.Product_Id = Product_Id;
                                var LogResponse = await _repository.datasyncservice.SaveDataSyncLog(logmodel);
                            }
                        }
                        else if (Sync_Type_Id == 3)
                        {

                            var userdata = await _repository.datasyncservice.GetUser_Data<VendorSyncModel>(item.Record_Id, Sync_Type_Id);
                            if (userdata != null && userdata.ResponseData != null && userdata.ResponseData.Any())
                            {
                                foreach (VendorSyncModel data in userdata.ResponseData)
                                {
                                    var usersyncresponse = await PostAsync<string>(Product_Api_Url + "DataSync/SyncVendors", data);
                                    DataSyncLogModel logmodel = new DataSyncLogModel();
                                    logmodel.Record_Id = item.Record_Id;
                                    logmodel.Sync_Type_Id = Sync_Type_Id;
                                    logmodel.Sync_Status = usersyncresponse.Status;
                                    logmodel.Message = usersyncresponse.Message;
                                    logmodel.Product_Id = Product_Id;
                                    var LogResponse = await _repository.datasyncservice.SaveDataSyncLog(logmodel);
                                }

                            }
                        }
                        else if (Sync_Type_Id > 3 && Sync_Type_Id < 10)
                        {

                            var userdata = await _repository.datasyncservice.GetUser_Data<LevelSyncModel>(item.Record_Id, Sync_Type_Id);
                            if (userdata != null && userdata.ResponseData != null && userdata.ResponseData.Any())
                            {
                                foreach (LevelSyncModel data in userdata.ResponseData)
                                {
                                    var usersyncresponse = await PostAsync<string>(Product_Api_Url + "DataSync/SyncLevels", data);
                                    DataSyncLogModel logmodel = new DataSyncLogModel();
                                    logmodel.Record_Id = item.Record_Id;
                                    logmodel.Sync_Type_Id = Sync_Type_Id;
                                    logmodel.Sync_Status = usersyncresponse.Status;
                                    logmodel.Message = usersyncresponse.Message;
                                    logmodel.Product_Id = Product_Id;
                                    var LogResponse = await _repository.datasyncservice.SaveDataSyncLog(logmodel);
                                }

                            }
                        }
                    }
                }
            }

        }
        public async Task<ResponseModel<T>> PostAsync<T>(string ApiUrl, object requestData)
        {

            ResponseModel<T> HostInfoData = null;
            try
            {
                var _client = new HttpClient();
                //_client.DefaultRequestHeaders.Authorization =
                //                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "");

                // ✅ Convert Object to JSON
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // ✅ POST Call
                var HostInfo = await _client.PostAsync(
                    ApiUrl,
                    content
                );

                if (HostInfo.IsSuccessStatusCode)
                {
                    var hostjson = await HostInfo.Content.ReadAsStringAsync();

                    HostInfoData = JsonSerializer.Deserialize<ResponseModel<T>>(
                        hostjson,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                await ValidationHelper.LogErrorToFile("Error in Data Sync API", ex, "API_Error_Log");
            }


            return HostInfoData;
        }





    }
}
