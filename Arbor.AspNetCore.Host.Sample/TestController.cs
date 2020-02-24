using System;
using System.Threading.Tasks;
using MediatR;
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

        [Route("~/notify")]
        [HttpGet]
        public async Task<IActionResult> Notify([FromServices] IMediator mediator)
        {
            await mediator.Publish(new TestNotificationA(Guid.NewGuid()));

            return Ok();
        }
        [Route("~/request")]
        [HttpGet]
        public async Task<IActionResult> Request([FromServices] IMediator mediator)
        {
            await mediator.Send(new TestRequest(Guid.NewGuid()));

            return Ok();
        }
        [Route("~/notify-indirect")]
        [HttpGet]
        public async Task<IActionResult> Notify([FromServices] IMediator mediator, [FromServices] UsingBackgroundService usingBackgroundService)
        {
            usingBackgroundService.CallOnBackgrounService();

            await mediator.Publish(new TestNotificationA(Guid.NewGuid()));

            return Ok();
        }

        [Route("~/notify-all")]
        [HttpGet]
        public async Task<IActionResult> NotifyAll([FromServices] IMediator mediator)
        {
            await mediator.Publish(new TestNotificationA(Guid.NewGuid()));
            await mediator.Publish(new TestNotificationB(Guid.NewGuid()));

            return Ok();
        }
    }
}