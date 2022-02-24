using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;
using FakeAuth.Tests;

namespace FakeAuth.IntegrationTests
{
	[Collection("Integration Tests")]
	public class Non_Manager_AccessTests : IDisposable
	{
		private readonly TestWebApplication _appUnderTest;
		public Non_Manager_AccessTests()
		{
			_appUnderTest = new TestWebApplication();
		}
		public void Dispose() => _appUnderTest.Dispose();

		[Fact]
		public async Task Should_Be_Able_To_Access_NonManager_Endpoint()
		{
			using var client = _appUnderTest.CreateClient();

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
			using var client = _appUnderTest.CreateClient();

			// Act
			var response = await client.GetAsync("/api/protected");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().BeEmpty();
		}

		[Fact]
		public async Task Should_Be_Able_To_Access_Manager_Endpoint_With_Http_Claims()
		{
			using var client = _appUnderTest.CreateClient();
			client.SetFakeAuthClaims(new Claim(ClaimTypes.Role, "Manager"));

			// Act
			var response = await client.GetAsync("/api/protected");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().NotBeNullOrEmpty();
		}
	}
}