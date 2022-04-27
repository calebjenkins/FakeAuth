using intigrationtests.SampleWeb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace FakeAuth.IntegrationTests
{
	public class TestWebApplication : WebApplicationFactory<Program>
	{
		public string? Host { get; set; }
		public string? AllowedHost { get; set; }

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);

			builder.ConfigureServices(services =>
			{
				services.Configure<HostRewriteSettings>(s =>
				{
					s.Host = Host;
				});
				services.Configure<FakeAuthOptions>(FakeAuthDefaults.SchemaName, opts =>
				{
					opts.AllowedHost = AllowedHost ?? FakeAuthOptions.DefaultAllowedHost;
				});
			});
		}
	}
}