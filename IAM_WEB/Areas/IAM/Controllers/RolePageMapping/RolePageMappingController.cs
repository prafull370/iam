using ADMIN_SERVICE.Common.Enums;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office.CustomUI;
using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Dropdown;
using IAM_WEB.Common.Helper.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.RolePageMapping;
using Newtonsoft.Json;

namespace IAM_WEB.Areas.IAM.Controllers.RolePageMapping
{
	[Area("IAM")]
	public class RolePageMappingController : Controller
	{
		private readonly IAPIHelper FactoryService;
		private readonly string BaseApi = "RolePageMapping";
		private readonly IDropdownHelper Dropdown;

		public RolePageMappingController(IAPIHelper aPIHelper, IDropdownHelper dropdown)
		{
			this.FactoryService = aPIHelper;
			this.Dropdown = dropdown;
		}
		public async Task<IActionResult> Index()
		{
			RolePageMappingRequestModel model = new RolePageMappingRequestModel();
			DDLRequestModel ddlmodel = new DDLRequestModel()
			{
				Flag = DDLShortcodeEnum.CommonDDL.ROLE.ToString()
			};
			ViewBag.RoleList = await Dropdown.Get(ddlmodel);
			return View(model);
		}
        public async Task<IActionResult> App()
        {
            AppRolePageMappingRequestModel model = new AppRolePageMappingRequestModel();
            DDLRequestModel ddlmodel = new DDLRequestModel()
            {
                Flag = DDLShortcodeEnum.CommonDDL.ROLE.ToString()
            };
            ViewBag.RoleList = await Dropdown.Get(ddlmodel);
            return View(model);
        }

        #region Save Form
        [HttpPost]
		public async Task<IActionResult> Save([FromBody] RolePageMappingRequestModel model)
		{
			var Validation = ValidationHelper.ValidateModel<RolePageMappingRequestModel>(model, ModelState);
			if (!Validation.Status)
			{
				return Json(Validation);
			}
			var result = await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
			return Json(result);
		}
		#endregion


		[HttpPost]
		public async Task<IActionResult> Get(int Role_Id)
		{
			var queryParams = new Dictionary<string, string>
			{
				["Role_Id"] = Role_Id.ToString(),
			};
			string url = QueryHelpers.AddQueryString(BaseApi + "/get", queryParams);
			var data = await FactoryService.GetAsync<List<RolePageMappingResponseModel>>(url);
			var result = data.ResponseData
			.Where(x => x.Product_Id != 0)
			.GroupBy(p => new { p.Product_Id, p.Product_Name, p.Product_Logo })
			.Select(productGroup => new ProductViewModel
			{
				Product_Id = productGroup.Key.Product_Id,
				Product_Name = productGroup.Key.Product_Name,
				Product_Logo = productGroup.Key.Product_Logo,

				Services = productGroup
					.Where(x => x.Service_Id != 0)
					.GroupBy(s => new { s.Service_Id, s.Service_Name, s.Service_Logo })
					.Select(serviceGroup => new ServiceViewModel
					{
						Service_Id = serviceGroup.Key.Service_Id,
						Service_Name = serviceGroup.Key.Service_Name,
						Service_Logo = serviceGroup.Key.Service_Logo,

						ParentPages = BuildPageTree(serviceGroup.Where(p => p.Module_Id == 0)),

						Modules = serviceGroup
							.Where(p => p.Module_Id != 0)
							.GroupBy(m => new { m.Module_Id, m.Module_Name, m.Module_Logo })
							.Select(moduleGroup => new ModuleViewModel
							{
								Module_Id = moduleGroup.Key.Module_Id,
								Module_Name = moduleGroup.Key.Module_Name,
								Module_Logo = moduleGroup.Key.Module_Logo,

								ParentPages = BuildPageTree(moduleGroup)
							}).ToList()
					}).ToList()
			}).ToList();


			if (data.ResponseData.Count > 0)
			{
				return PartialView("_RolePagePartial", result);
			}
			return Json(data);
		}

		// Deepest page level the mapping screen supports (Level 1 = root page).
		private const int MaxPageLevel = 5;

		// Builds the page hierarchy (up to MaxPageLevel levels) from the flat SP rows by
		// Parent_Page_Id, the same way the side menu builds it. Is_Parent is not used for
		// structure because an intermediate page can be a parent and have a parent itself.
		// A page whose parent is not in the same service/module (e.g. legacy 2147483647
		// sentinel) is shown as a root page instead of being dropped.
		private static List<PageViewModel> BuildPageTree(IEnumerable<RolePageMappingResponseModel> rows)
		{
			var pages = rows
				.Where(p => p.Page_Id != 0)
				.GroupBy(p => p.Page_Id)
				.Select(g => g.First())
				.ToList();

			var pageIds = pages.Select(p => p.Page_Id).ToHashSet();
			bool HasParentInSet(RolePageMappingResponseModel p) =>
				p.Parent_Page_Id != p.Page_Id && pageIds.Contains(p.Parent_Page_Id);

			var childrenByParent = pages.Where(HasParentInSet).ToLookup(p => p.Parent_Page_Id);

			List<PageViewModel> Build(IEnumerable<RolePageMappingResponseModel> level, int depth) =>
				level.Select(p => new PageViewModel
				{
					Page_Id = p.Page_Id,
					Page_Name = p.Page_Name,
					Page_Logo = p.Page_Logo,
					Is_Parent = p.Is_Parent,
					Is_Landing_Page = p.Is_Landing_Page,
					Is_Page_Mapped = p.Is_Page_Mapped,
					Page_Level = depth,
					ChildPages = depth < MaxPageLevel
						? Build(childrenByParent[p.Page_Id], depth + 1)
						: new List<PageViewModel>()
				}).ToList();

			return Build(pages.Where(p => !HasParentInSet(p)), 1);
		}
        [HttpPost]
        public async Task<IActionResult> AppGet(int Role_Id)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Role_Id"] = Role_Id.ToString(),
            };

            string url = QueryHelpers.AddQueryString(BaseApi + "/appget", queryParams);

            var data = await FactoryService
                .GetAsync<List<AppRolePageMappingResponseModel>>(url);

            var result = data.ResponseData
                .Where(x => x.App_Id != 0)
                .GroupBy(a => new { a.App_Id, a.App_Name, a.App_Logo })
                .Select(appGroup => new AppViewModel
                {
                    App_Id = appGroup.Key.App_Id,
                    App_Name = appGroup.Key.App_Name,
                    App_Logo = appGroup.Key.App_Logo,

                    Services = appGroup
                        .Where(s => s.Service_Id != 0)
                        .GroupBy(s => new { s.Service_Id, s.Service_Name, s.Service_Logo })
                        .Select(serviceGroup => new AppServiceViewModel
                        {
                            Service_Id = serviceGroup.Key.Service_Id,
                            Service_Name = serviceGroup.Key.Service_Name,
                            Service_Logo = serviceGroup.Key.Service_Logo,

                            ParentPages = serviceGroup
                                .Where(p => p.Is_Parent)
                                .Select(parent => new AppPageViewModel
                                {
                                    Page_Id = parent.Page_Id,
                                    Page_Name = parent.Page_Name,
                                    Page_Logo = parent.Page_Logo,
                                    Is_Parent = parent.Is_Parent,
                                    Is_Landing_Page = parent.Is_Landing_Page,
                                    Is_Page_Mapped = parent.Is_Page_Mapped,

                                    ChildPages = serviceGroup
                                        .Where(child =>
                                            !child.Is_Parent &&
                                            child.Parent_Page_Id == parent.Page_Id)
                                        .Select(child => new AppPageViewModel
                                        {
                                            Page_Id = child.Page_Id,
                                            Page_Name = child.Page_Name,
                                            Page_Logo = child.Page_Logo,
                                            Is_Parent = child.Is_Parent,
                                            Is_Landing_Page = child.Is_Landing_Page,
                                            Is_Page_Mapped = child.Is_Page_Mapped
                                        })
                                        .ToList()
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            if (result.Any())
            {
                return PartialView("_AppRolePagePartial", result);
            }

            return Json(data);
        }
        #region Save Form
        [HttpPost]
        public async Task<IActionResult> AppSave([FromBody] AppRolePageMappingRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<AppRolePageMappingRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PostAsync<string>(BaseApi + "/appsave", model, "");
            return Json(result);
        }
        #endregion
    }
}
