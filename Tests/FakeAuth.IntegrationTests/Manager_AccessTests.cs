using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using FakeAuth.Tests;

namespace FakeAuth.IntegrationTests
{
	// Added Collection Attribute so that our tests aren't run in parallel
	[Collection("Integration Tests")]
	public class Manager_AccessTests :IDisposable
	{
		private readonly HttpClient _client;
		private readonly TestWebApplication _appUnderTest;

		public Manager_AccessTests()
		{
			_appUnderTest = new TestWebApplication();
			_client = _appUnderTest.CreateClient();

			_client.SetFakeAuthClaims(
				new Claim(ClaimTypes.Name, "Joe Manager"),
				new Claim(ClaimTypes.Role, "Manager")
			);
		}


		[Fact]
		public async Task Should_Be_Able_To_Access_NonManager_Endpoint()
		{
			// Act
			var response = await _client.GetAsync("/api/open");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task Should_Also_Be_Able_To_Access_Manager_Endpoint()
		{
			// Act
			var response = await _client.GetAsync("/api/protected");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().NotBeNullOrEmpty();
		}

		public void Dispose()
		{
			_client?.Dispose();
			_appUnderTest?.Dispose();
		}
	}
}