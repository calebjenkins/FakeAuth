using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace intigrationtests.SampleWeb.Controllers
{
	[ApiController]
	public class ApiController : ControllerBase
	{
		private readonly IHttpContextAccessor _http;
		public ApiController(IHttpContextAccessor Http)
		{
			_http = Http ?? throw new ArgumentNullException(nameof(Http));
		}

		[HttpGet]
		[Route("api/open")]
		public JsonResult Open()
		{
			return processRequest();
		}

		[HttpGet]
		[Route("api/protected")]
		[Authorize(Roles = "Manager")]
		public JsonResult RoleProtected()
		{
			return processRequest();
		}

		private JsonResult processRequest()
		{
			var claims = _http?.HttpContext?.User?.Claims;
			var result = new Dictionary<string, string>();
			foreach (var claim in claims)
			{
				result.Add(claim.Type, claim.Value);
			}

			return new JsonResult(result);
		}
	}
}