using ADMIN_SERVICE.Common.Enums;
using DocumentFormat.OpenXml.Office.CustomUI;
using IAM_API.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Vendor;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Vendor
{
    [Area("IAM")]
    public class VendorController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly string BaseApi = "Vendor";
		private readonly IDropdownHelper Dropdown;
        private readonly ICryptographyService _cryptography;

        public VendorController(IAPIHelper aPIHelper, IDropdownHelper dropdown,ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            this.Dropdown = dropdown;
            this._cryptography = cryptography;
        }
        #region List
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
            var result = await FactoryService.PostAsync<List<VendorResponseModel>>(BaseApi + "/get", pagination, "");
            List<VendorResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Vendor_Id = _cryptography.Encrypt(c.Vendor_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        #endregion

        #region Add Vendor
        public async Task<IActionResult> Add()
        {
			DDLRequestModel ddlmodel = new DDLRequestModel();
            ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.COUNTRY.ToString();
			ViewBag.CountryList = await Dropdown.Get(ddlmodel);
			VendorRequestModel model = new VendorRequestModel();
            return View("~/Areas/IAM/Views/Vendor/Add.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(VendorRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<VendorRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
        #endregion

        #region Edit Vendor
        public async Task<IActionResult> Edit(string Id)
        {
			DDLRequestModel ddlmodel = new DDLRequestModel();
			ddlmodel.Flag = DDLShortcodeEnum.CommonDDL.COUNTRY.ToString();
			ViewBag.CountryList = await Dropdown.Get(ddlmodel);
			var Result = await Get_By_Id<VendorRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            return View("~/Areas/IAM/Views/Vendor/Add.cshtml", Result.ResponseData);
        }

        public async Task<ResponseModel<T>> Get_By_Id<T>(int Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Vendor_Id"] = Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }
        #endregion

        #region Update Vendor Details
        [HttpPost]
        public async Task<IActionResult> Update(VendorRequestModel model)
        {
            
            var Validation = ValidationHelper.ValidateModel<VendorRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        #endregion

        #region Vendor Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<VendorResponseModel>(Id);
            return PartialView("_VendorInfo",Result.ResponseData);
        }
        #endregion
    }
}
