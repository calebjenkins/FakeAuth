using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace FakeAuth.IntegrationTests
{
	public class TestWebApplication : WebApplicationFactory<Program>
	{
		public List<Claim> DefaultClaims { get; set; } = new ();
		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				services.ConfigureFakeAuthClaims(DefaultClaims.ToArray());

			});
			// shared extra set up goes here
			return base.CreateHost(builder);
		}

		// Alternative approach, doesn't require a wrapper class:
		//	var application = new WebApplicationFactory<Program>()
		//	.WithWebHostBuilder(builder =>
		//	{
		//		builder.ConfigureServices(services =>
		//		{
		//			// Set up Services
		//		});
		//  });
	}
}