
using IAM_MODEL.DeviceReset;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Controllers.IAM.DeviceReset
{
    [Area("IAM")]
    public class DeviceResetController : Controller
    {

        #region Declaration
        private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "DeviceReset";
        public DeviceResetController(IAPIHelper aPIHelper,  ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            _cryptography = cryptography;
        }
        #endregion

        #region Device List 
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(string? Search_Val)
        {
            PaginationModel pagination = new PaginationModel();
            if (Search_Val != null)
            {
                pagination.Search_Val = Search_Val;
            }
            PaginationHelper.PagingData(Request.Form, ref pagination);
            var result = await FactoryService.PostAsync<List<DeviceResetModel>>(BaseApi + "/get", pagination, "");
            List<DeviceResetModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Device_Id = _cryptography.Encrypt(c.Device_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        #endregion

        #region User Device Update
        [HttpPost]
        public async Task<IActionResult> UpdateDeviceId([FromBody] UpdateDeviceId model)
        {

            var Validation = ValidationHelper.ValidateModel<UpdateDeviceId>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        #endregion
    }
}
