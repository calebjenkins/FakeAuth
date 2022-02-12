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
			//		services.ConfigureFakeAuthClaims(
			//			new Claim(ClaimTypes.Name, "Joe Manager"),
			//			new Claim(ClaimTypes.Role, "Manager")
			//		});
			//	});
			//});

			_appUnderTest = new TestWebApplication
			{
				DefaultClaims =
				{
					new Claim(ClaimTypes.Name, "Joe Manager"),
					new Claim(ClaimTypes.Role, "Manager"),
				}
			};
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