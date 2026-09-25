using Microsoft.AspNetCore.Mvc;

namespace IAM_WEB.Areas.IAM.Controllers.Dashboard
{
	[Area("IAM")]
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
