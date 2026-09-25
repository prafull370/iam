using ADMIN_SERVICE.Common.Enums;
using DocumentFormat.OpenXml.Office.CustomUI;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.RolePageMapping;
using ModelLayer.Models.IAM.UserDeviceMapping;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.UserDeviceMapping
{
    [Area("IAM")]
    public class UserDeviceMappingController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "UserDeviceMapping";
        private readonly IDropdownHelper Dropdown;
        private readonly IConfiguration _config;

        public UserDeviceMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown, IConfiguration config)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdown;
            _config = config;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.User_List = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.REPORTING.ToString() });
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Get(int User_Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["User_Id"] = User_Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/get", queryParams);
            var data = await FactoryService.GetAsync<List<UserDeviceMappingResponseModel>>(url);
            var result = data.ResponseData;
            if (data?.ResponseData?.Count > 0)
            {
                return PartialView("_User_Device", result);
            }
            //return Json(data);
            return Content("");
        }
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] UserDeviceMappingRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<UserDeviceMappingRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }

    }
}
