using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace FakeAuth.IntegrationTests
{
	public class TestWebApplication : WebApplicationFactory<Program>
	{
		protected override IHost CreateHost(IHostBuilder builder)
		{
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