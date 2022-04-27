using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace FakeAuth.IntegrationTests;

[Collection("Integration Tests")]
public sealed class NonLocalhostTests
{
	[Fact]
	public async Task ThrowsOnNonLocalhost()
	{
		using var app = new TestWebApplication
		{
			Host = "example.com",
		};
		using var client = app.CreateClient();

		var result = await client.GetAsync("/api/open");

		result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}

	[Fact]
	public async Task AllowsNonLocalhostWhenConfigured()
	{
		using var app = new TestWebApplication
		{
			Host = "example.com",
			AllowedHost = "example.com",
		};
		using var client = app.CreateClient();

		var result = await client.GetAsync("/api/open");

		result.StatusCode.Should().Be(HttpStatusCode.OK);
	}
}