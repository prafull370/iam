
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Action;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Action
{
	[Area("IAM")]
	public class ActionController : Controller
	{
		private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "Actions";

		public ActionController(IAPIHelper aPIHelper,ICryptographyService cryptography)
        {
			this.FactoryService = aPIHelper;
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
			var result = await FactoryService.PostAsync<List<ActionResponseModel>>(BaseApi + "/get", pagination, "");
			List<ActionResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Action_Id = _cryptography.Encrypt(c.Action_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
		}
        #endregion

        #region Add Action
        public IActionResult Add()
		{
            ActionRequestModel model = new ActionRequestModel();
			return View("~/Areas/IAM/Views/Action/Add.cshtml", model);
		}

		[HttpPost]
        public async Task<IActionResult> Save(ActionRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<ActionRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
        #endregion
        #region Edit Action
        public async Task<IActionResult> Edit(string Id)
        {
            var Result = await Get_By_Id<ActionRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            return View("~/Areas/IAM/Views/Action/Add.cshtml", Result.ResponseData);
        }

        public async Task<ResponseModel<T>> Get_By_Id<T>(int Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Action_Id"] = Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }
        #endregion

        #region Update Action Details
        [HttpPost]
        public async Task<IActionResult> Update(ActionRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<ActionRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        #endregion

        #region Action Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<ActionResponseModel>(Id);
            return Json(Result);
        }
        #endregion
    }
}
