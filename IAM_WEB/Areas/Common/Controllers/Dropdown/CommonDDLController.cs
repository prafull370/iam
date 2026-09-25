using IAM_WEB.Common.Helper.Dropdown;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.Common;

namespace IAM_WEB.Areas.Common.Controllers.Dropdown
{
    [Area("IAM")]
    public class CommonDDLController : Controller
    {
		private readonly IDropdownHelper FactoryService;

		public CommonDDLController(IDropdownHelper dropdownHelper)
        {
			this.FactoryService = dropdownHelper;
		}


        [HttpPost]
        public async Task<IActionResult> Get(DDLRequestModel model)
        {
            var result = await FactoryService.Get(model);

			return Json(result);
        }
	}
}
