using Microsoft.AspNetCore.Mvc;

namespace Arbor.AspNetCore.Host.Sample.Areas
{
    [Area("TestArea")]
    public class ExampleController : Controller
    {
        [Route("~/in-area")]
        [HttpGet]
        public IActionResult Index() => View();

    }
}