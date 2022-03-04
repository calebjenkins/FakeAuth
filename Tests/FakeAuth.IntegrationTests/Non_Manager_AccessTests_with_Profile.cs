using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;
using System.Net.Http;
using FakeAuth.Testing;

namespace FakeAuth.IntegrationTests
{
	[Collection("Integration Tests")]
	public class Non_Manager_AccessTests_with_Profile : IDisposable
	{
		private readonly TestWebApplication _appUnderTest;
		private readonly HttpClient _client; 

		public Non_Manager_AccessTests_with_Profile()
		{
			_appUnderTest = new TestWebApplication();
			_client = _appUnderTest.CreateClient();

			_client.SetFakeAuthClaimns<NonManagerJoeProfile>();
		}
		
		public void Dispose()
		{
			_client?.Dispose();
			_appUnderTest?.Dispose();
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
		public async Task Should_Not_Be_Able_To_Access_Manager_Endpoint()
		{
			// Act
			var response = await _client.GetAsync("/api/protected");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().BeEmpty();
		}

		[Fact]
		public async Task Should_Be_Able_To_Access_Manager_Endpoint_With_Http_Claims()
		{
			_client.SetFakeAuthClaims(new Claim(ClaimTypes.Role, "Manager"));

			// Act
			var response = await _client.GetAsync("/api/protected");

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var content = await response.Content.ReadAsStringAsync();
			content.Should().NotBeNullOrEmpty();
		}
	}
}