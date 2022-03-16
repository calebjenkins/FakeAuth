using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace FakeAuth.IntegrationTests
{
	public class TestWebApplication : WebApplicationFactory<Program>
	{
	}
}