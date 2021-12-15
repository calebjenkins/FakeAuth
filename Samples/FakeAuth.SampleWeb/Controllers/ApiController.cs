using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace FakeAuth.SampleWeb.Controllers
{
	[Authorize]
	public class ApiController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public ApiController(ILogger<HomeController> logger)
		{
			_logger = logger ?? throw new ArgumentException(nameof(logger));
		}

		[Authorize(Roles="Buyer")]
		[Route("api/buy")]
		[HttpGet]
		public JsonResult Buy()
		{
			var response = new { status = "ok", Message = "This endpoint is lockdown to specific roles" };
			return Json(response);
		}

		[Route("api/open")]
		[HttpGet]
		public JsonResult Open()
		{
			var response = new { status = "ok", Message = "This endpoint is open" };
			return Json(response);
		}
	}
}
