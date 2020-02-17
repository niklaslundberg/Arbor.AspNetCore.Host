using Microsoft.AspNetCore.Mvc;

namespace Arbor.AspNetCore.Host.Sample
{
    public class TestController : Controller
    {
        [Route("~/")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}