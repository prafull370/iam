using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DataSyncing;
using ModelLayer.Models.IAM.User;

namespace IAM_WEB.Areas.IAM.Controllers.DataSyncing
{
    [Area("IAM")]
    public class DataSyncController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IDropdownHelper Dropdown;
        private readonly IMemoryCache _cache;
        private readonly string BaseApi = "DataSync";
        public DataSyncController(IAPIHelper aPIHelper,IDropdownHelper dropdownHelper,IMemoryCache cache)
        {
            FactoryService=aPIHelper;
            Dropdown=dropdownHelper;
            _cache = cache;


        }
        public async Task<IActionResult> Index()
        {
            DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.PRODUCT.ToString();
            ViewBag.ProductList = await Dropdown.Get(ddlmodel);
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.SYNCTYPE.ToString();
            ViewBag.SyncTypeList = await Dropdown.Get(ddlmodel);
            return View();
        }
        #region Get List
        [HttpPost]
        public async Task<IActionResult> Get(DataSyncRequestModel model)
        {
            model.Flag = "LIST";
            PaginationModel pagination = new PaginationModel();
            PaginationHelper.PagingData(Request.Form, ref pagination);
            var result = await FactoryService.PostAsync<List<DataSyncResponseModel>>(BaseApi + "/get", model, "");
            List<DataSyncResponseModel> data = result.ResponseData;
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });

        }
        #endregion
        #region Sync Data
        [HttpPost]
        public async Task<IActionResult> SyncData(DataSyncRequestModel model)
        {
            model.Flag = "ALL";
            var result = await FactoryService.PostAsync<List<DataSyncResponseModel>>(BaseApi + "/syncdata", model, "");
            
            return Json(result);

        }
        #endregion
        private async Task ProcessDataSyncing(List<DataSyncResponseModel> model, int Sync_Type_Id, int Product_Id,string Token_Id)
        {
            //var products = HttpContext.Items["UserProductInfo"] as List<UserProductPermissionModel>;
            var UserProductInfo = _cache.Get<List<UserProductPermissionModel>>("USER_PRODUCT_INFO_" + Token_Id);
            string Product_Api_Url = UserProductInfo.Where(x => x.Product_Id == Product_Id).Select(x => x.Product_Api_Url).FirstOrDefault();
            foreach (var item in model)
            {
                if (Sync_Type_Id == 1)
                {
                    var queryParams = new Dictionary<string, string>
                    {
                        ["User_Id"] = Convert.ToString(item.Record_Id),
                        ["Sync_type_Id"] = Sync_Type_Id.ToString(),
                    };
                    string url = QueryHelpers.AddQueryString(BaseApi + "/getuserdataforsync", queryParams);

                    
                    var userdata = await FactoryService.GetAsync<List<UserSyncModel>>(url, "");
                    if (userdata!=null && userdata.ResponseData != null && userdata.ResponseData.Any())
                    {
                        foreach (var data in userdata.ResponseData)
                        {
                            var usersyncresponse = await FactoryService.PostAsync<string>(Product_Api_Url + "/SyncUser", userdata.ResponseData, "");
                            DataSyncLogModel logmodel = new DataSyncLogModel();
                            logmodel.Record_Id = item.Record_Id;
                            logmodel.Sync_Type_Id = Sync_Type_Id;
                            logmodel.Sync_Status = usersyncresponse.Status;
                            logmodel.Message = usersyncresponse.Message;
                            logmodel.Product_Id = Product_Id;
                            var LogResponse = await FactoryService.PostAsync<string>(BaseApi + "/savedatasynclog", logmodel, "");
                        }

                    }
                }
            }
        }
    }
}
