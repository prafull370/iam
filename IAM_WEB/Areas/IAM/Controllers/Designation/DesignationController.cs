using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Designation;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Designation
{
    [Area("IAM")]
    public class DesignationController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly IDropdownHelper Dropdown;
        private readonly string BaseApi = "Designation";
        private readonly ICryptographyService _cryptography;

        public DesignationController(IAPIHelper aPIHelper,IDropdownHelper dropdownHelper,ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;    
            this.Dropdown = dropdownHelper;
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
            var result = await FactoryService.PostAsync<List<DesignationResponseModel>>(BaseApi + "/get", pagination, "");
            List<DesignationResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Designation_Id = _cryptography.Encrypt(c.Designation_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
        #endregion

        #region Add Designation
        public async Task<IActionResult> Add()
        {
            DDLRequestModel dropdownmodel = new DDLRequestModel();
			dropdownmodel.Flag = "Department";
			ViewBag.DepartmentList = await Dropdown.Get(dropdownmodel);
            DesignationRequestModel model = new DesignationRequestModel();
            return View("~/Areas/IAM/Views/Designation/Add.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(DesignationRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<DesignationRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }
		#endregion


		#region Edit Designations
		public async Task<IActionResult> Edit(string Id)
		{
			DDLRequestModel dropdownmodel = new DDLRequestModel()
            {
                Flag = "Department"
            };
			ViewBag.DepartmentList = await Dropdown.Get(dropdownmodel);
			var Result = await Get_By_Id<DesignationRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
			return View("~/Areas/IAM/Views/Designation/Add.cshtml", Result.ResponseData);
		}

		public async Task<ResponseModel<T>> Get_By_Id<T>(int Id)
		{
			var queryParams = new Dictionary<string, string>
			{
				["Designation_Id"] = Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

			return await FactoryService.GetAsync<T>(url, "");
		}

		#endregion

		#region Update Designation Details
		[HttpPost]
        public async Task<IActionResult> Update(DesignationRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<DesignationRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        #endregion

        #region Designation Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<DesignationResponseModel>(Id);
            return Json(Result);
        }
        #endregion
    }
}
