using System.Text.Json;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DataSyncing;
using RepositoryLayer.IAM.IDataSyncing;

namespace DataLayer.IAM.DataSyncing
{
    public class DataSyncOrchestrator : IDataSyncOrchestrator
    {
        private readonly RepositoryDependencyService _repository;
        private readonly HttpClient _client;

        public DataSyncOrchestrator(
            RepositoryDependencyService repository,
        HttpClient client)
        {
            _repository=repository;
            _client = client;
        }
        public async Task ProcessDataSyncing(List<DataSyncResponseModel> model, int Sync_Type_Id, int Product_Id)
        {
            var product_data = await _repository.datasyncservice.Get_Product_Data(Product_Id);

            if (product_data != null && product_data.ResponseData != null && product_data.ResponseData.Any())
            {
                foreach (var product in product_data.ResponseData)
                {
                    string Product_Api_Url = "" + product.Product_API_Url;
                    if(product!=null && !string.IsNullOrEmpty(product.Product_API_Url))
                    {
                        var apiresponse = await PostAsync<string>(Product_Api_Url + "DataSync/TestSyncAPI", "Data");
                        if (apiresponse != null && apiresponse.Status)
                        {
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
                                            if (usersyncresponse != null)
                                            {
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
                                        if (usersyncresponse != null)
                                        {
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
                                else if (Sync_Type_Id == 3)
                                {

                                    var userdata = await _repository.datasyncservice.GetUser_Data<VendorSyncModel>(item.Record_Id, Sync_Type_Id);
                                    if (userdata != null && userdata.ResponseData != null && userdata.ResponseData.Any())
                                    {
                                        foreach (VendorSyncModel data in userdata.ResponseData)
                                        {
                                            var usersyncresponse = await PostAsync<string>(Product_Api_Url + "DataSync/SyncVendors", data);
                                            if (usersyncresponse!= null)
                                            {
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
                                else if (Sync_Type_Id > 3 && Sync_Type_Id < 10)
                                {

                                    var userdata = await _repository.datasyncservice.GetUser_Data<LevelSyncModel>(item.Record_Id, Sync_Type_Id);
                                    if (userdata != null && userdata.ResponseData != null && userdata.ResponseData.Any())
                                    {
                                        foreach (LevelSyncModel data in userdata.ResponseData)
                                        {
                                            var usersyncresponse = await PostAsync<string>(Product_Api_Url + "DataSync/SyncLevels", data);
                                            if (usersyncresponse != null)
                                            {
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

            }


            return HostInfoData;
        }
    }

}
