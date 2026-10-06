using ADMIN_SERVICE.Common.Enums;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Designation;
using ModelLayer.Models.IAM.Discom;
using ModelLayer.Models.IAM.DiscomProductMapping;
using ModelLayer.Models.IAM.LevelHierarchy;

namespace IAM_WEB.Areas.IAM.Controllers.LevelHierarchy
{
	[Area("IAM")]
	public class LevelHierarchyController : Controller
	{
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "LevelHierarchy";
		private readonly IDropdownHelper Dropdown;
		public LevelHierarchyController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
        {
			this.FactoryService = aPIHelper;
			this.Dropdown = dropdown;
		}

		#region Save Form
        public async Task<IActionResult> Add()
		{
			ViewBag.ZoneList = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.ZONE.ToString() });
			ViewBag.Level = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL.ToString() });
            ViewBag.Level1 = (await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL1.ToString() }))?.FirstOrDefault()?.Name ?? "Level1";
            ViewBag.Level2 = (await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL2.ToString() }))?.FirstOrDefault()?.Name ?? "Level2";
            ViewBag.Level3 = (await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL3.ToString() }))?.FirstOrDefault()?.Name ?? "Level3";
            ViewBag.Level4 = (await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL4.ToString() }))?.FirstOrDefault()?.Name ?? "Level4";
            ViewBag.Level5 = (await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL5.ToString() }))?.FirstOrDefault()?.Name ?? "Level5";
            ViewBag.LEVEL6LB = (await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL6LB.ToString() }))?.FirstOrDefault()?.Name ?? "Level6";
            LevelHierarchyRequestModel model = new LevelHierarchyRequestModel();
			return View("~/Areas/IAM/Views/LevelHierarchy/Add.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> Save(LevelHierarchyRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<LevelHierarchyRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion

		#region Level Hierarchy List
		public async Task<IActionResult> Index()
		{
            ViewBag.ZoneList = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.ZONE.ToString() });
			ViewBag.Level = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL.ToString() });
			return View("~/Areas/IAM/Views/LevelHierarchy/Index.cshtml");
		}

        [HttpPost]
        public async Task<IActionResult> Get(string? Level_Type)
        {
            PaginationModel pagination = new PaginationModel();

            // Step 2: Call fixed paging helper
            PaginationHelper.PagingData(Request.Form, ref pagination);

            // Step 3: Now copy values to extended model
            GetLevelHierarchyModel model = new GetLevelHierarchyModel
            {
                Start = pagination.Start,
                Length = pagination.Length,
                SortColumn = pagination.SortColumn,
                SortColumnDirection = pagination.SortColumnDirection,
                Level_Type = Level_Type
            };

            // Step 4: Call the API
            var result = await FactoryService.PostAsync<List<LevelHierarchyResponseModel>>(BaseApi + "/get", model, "");

            // Step 5: Return formatted JSON for DataTables
            List<LevelHierarchyResponseModel> data = result.ResponseData;
            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                data
            });
        }
		#endregion

		#region Edit Hierarchy
		public async Task<IActionResult> Edit(string Id, string Name)
		{
			var idString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Id));
			var name = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Name));

			int id = Convert.ToInt32(Uri.UnescapeDataString(idString));
			string levelName = Uri.UnescapeDataString(name);

			ViewBag.ZoneList = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.ZONE.ToString() });
			ViewBag.Level = await Dropdown.Get(new DDLRequestModel { Flag = DDLShortcodeEnum.CommonDDL.LEVEL.ToString() });
			LevelHierarchyGetModel model = new LevelHierarchyGetModel();
			model.Level_Id = id;
			model.Level_Type = levelName;
			var Result = await FactoryService.PostAsync<LevelHierarchyRequestModel>(BaseApi + "/getbyid", model, "");
			return View("~/Areas/IAM/Views/LevelHierarchy/Add.cshtml", Result.ResponseData);
		}
		#endregion

		#region Update Hierarchy
		[HttpPost]
		public async Task<IActionResult> Update(LevelHierarchyRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<LevelHierarchyRequestModel>(model, ModelState);
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
