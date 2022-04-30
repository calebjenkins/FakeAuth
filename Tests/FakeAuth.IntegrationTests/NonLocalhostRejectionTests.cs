using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace FakeAuth.IntegrationTests;

[Collection("Integration Tests")]
public sealed class NonLocalhostTests
{
	[Theory]
	[InlineData("localhost", HttpStatusCode.OK)]
	[InlineData("example.com", HttpStatusCode.Unauthorized)]
	public async Task ThrowsOnNonLocalhostByDefault(string host, HttpStatusCode expected)
	{
		using var app = new TestWebApplication
		{
			Host = host,
		};
		using var client = app.CreateClient();

		var result = await client.GetAsync("/api/open");

		result.StatusCode.Should().Be(expected);
	}

	[Theory]
	[InlineData("example.com", HttpStatusCode.OK)]
	[InlineData("foobar.com", HttpStatusCode.OK)]
	[InlineData("localhost", HttpStatusCode.OK)]
	[InlineData("google.com", HttpStatusCode.Unauthorized)]
	public async Task AllowsNonLocalhostWhenConfigured(string host, HttpStatusCode expected)
	{
		using var app = new TestWebApplication
		{
			Host = host,
			AllowedHosts = new[] { "example.com", "foobar.com", "localhost" },
		};
		using var client = app.CreateClient();

		var result = await client.GetAsync("/api/open");

		result.StatusCode.Should().Be(expected);
	}
}