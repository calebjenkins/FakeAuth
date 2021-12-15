using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Net;

namespace FakeAuth.IntegrationTests
{
	[Collection("Integration Tests")]
	public class Non_Manager_AccessTests : IDisposable
	{
		private readonly TestWebApplication _appUnderTest;
		public Non_Manager_AccessTests()
		{
			// Set up - Sets Authorization to use the default FakeAuth Profile
			Program.BuildAuth = new Action<WebApplicationBuilder>((builder) =>
			{
				builder.Services.UseFakeAuth();
			});

			_appUnderTest = new TestWebApplication();
		}
		public void Dispose() => _appUnderTest.Dispose();

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
		public async Task Should_Not_Be_Able_To_Access_Manager_Endpoint()
		{
			var client = _appUnderTest.CreateClient();

			// Act
			var response = await client.GetAsync("/api/protected");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().BeEmpty();
		}
	}
}