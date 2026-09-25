using Microsoft.AspNetCore.Mvc;

namespace IAM_WEB.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult Error500()
        {
            return View();
        }
        public IActionResult Error503()
        {
            return View();
        }
        public IActionResult Error505()
        {
            return View();
        }
        public IActionResult Maintenance()
        {
            return View();
        }
        public IActionResult Error403()
        {
            return View();
        }
    }
}
