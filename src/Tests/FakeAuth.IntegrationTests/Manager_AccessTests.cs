using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;

namespace FakeAuth.IntegrationTests
{
	// Added Collection Attribute so that our tests aren't run in parallel
	[Collection("Integration Tests")]
	public class Manager_AccessTests :IDisposable
	{
		private readonly TestWebApplication _appUnderTest;

		public Manager_AccessTests()
		{
			// Alternative approach, doesn't require a wrapper class:
			//_appUnderTest = new WebApplicationFactory<Program>()
			//.WithWebHostBuilder(builder =>
			//{
			//	builder.ConfigureServices(services =>
			//	{
			//		services.UseFakeAuth((options) =>
			//		{
			//			options.Claims.Add(new Claim(ClaimTypes.Name, "Joe Manager"));
			//			options.Claims.Add(new Claim(ClaimTypes.Role, "Manager"));
			//		});
			//	});
			//});
			// Set up - Sets Authorization to use the default FakeAuth Profile
			Program.BuildAuth = new Action<WebApplicationBuilder>((builder) =>
			{
				// Needed for the protected endpoint (Role = Manager)
				builder.Services.UseFakeAuth((options) =>
				{
					options.Claims.Add(new Claim(ClaimTypes.Name, "Joe Manager"));
					options.Claims.Add(new Claim(ClaimTypes.Role, "Manager"));
				});
			});

			_appUnderTest = new TestWebApplication();
		}


		[Fact]
		public async Task Should_Be_Able_To_Access_NonManager_Endpoint()
		{
			var client = _appUnderTest.CreateClient();

			// Act
			var response = await client.GetAsync("/api/open");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task Should_Also_Be_Able_To_Access_Manager_Endpoint()
		{
			var client = _appUnderTest.CreateClient();

			// Act
			var response = await client.GetAsync("/api/protected");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().NotBeNullOrEmpty();
		}

		public void Dispose() => _appUnderTest.Dispose();
	}
}