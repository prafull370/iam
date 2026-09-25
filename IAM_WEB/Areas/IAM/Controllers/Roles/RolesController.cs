using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Company;
using ModelLayer.Models.IAM.Role;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Roles
{
	[Area("IAM")]
	public class RolesController : Controller
	{
		private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "Role";

		public RolesController(IAPIHelper aPIHelper,ICryptographyService cryptography)
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
			var result = await FactoryService.PostAsync<List<RoleResponseModel>>(BaseApi + "/get", pagination, "");
			List<RoleResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Role_Id = _cryptography.Encrypt(c.Role_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
		}
		#endregion

		#region Add Role
		public IActionResult Add()
		{
			RoleRequestModel model = new RoleRequestModel();
			return View("~/Areas/IAM/Views/Roles/Add.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> Save(RoleRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<RoleRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
        #endregion

        #region Edit Role
        public async Task<IActionResult> Edit(string Id)
        {
            var Result = await Get_By_Id<RoleRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            return View("~/Areas/IAM/Views/Roles/Add.cshtml", Result.ResponseData);
        }

        public async Task<ResponseModel<T>> Get_By_Id<T>(int Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Role_Id"] = Id.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }
        #endregion

        #region Update Role Details
        [HttpPost]
        public async Task<IActionResult> Update(RoleRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<RoleRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }
        #endregion

        #region Role Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<RoleResponseModel>(Id);
            return Json(Result);
        }
        #endregion
    }
}
