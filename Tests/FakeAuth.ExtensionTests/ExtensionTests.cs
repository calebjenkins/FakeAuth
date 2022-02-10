using FakeAuth.Profiles;
using FluentAssertions;
using System.Security.Claims;
using Xunit;

namespace FakeAuth.ExtensionTests;

public class ExtensionTests
{
	[Fact]
	public void ExtensionShouldReturnAllClaims()
	{
		var profile = new DefaultProfile();
		var claims = profile.GetClaims();

		claims.Count.Should().Be(2);
	}

	[Fact]
	public void ExtensionShouldReturnAllClaimKeys()
	{
		var profile = new DefaultProfile();
		var keys = profile.GetClaimKeys();

		keys.Count.Should().Be(2);
	}

	[Fact]
	public void ExtensionShould_only_return_unique_ClaimKeys()
	{
		var profile = new FakeTestProfile();
		var keys = profile.GetClaimKeys();

		keys.Count.Should().Be(4);
	}


}

public class FakeTestProfile : IFakeAuthProfile
{
	public Action<FakeAuthOptions> OptionBuilder()
	{
		return new Action<FakeAuthOptions>(x =>
						  {
							  x.Claims.Add(new Claim(ClaimTypes.Name, "Test 1"));
							  x.Claims.Add(new Claim(ClaimTypes.Email, "test@test.com"));
							  x.Claims.Add(new Claim(ClaimTypes.Email, "test2email.com"));
							  x.Claims.Add(new Claim(ClaimTypes.Role, "Manager"));
							  x.Claims.Add(new Claim(ClaimTypes.Role, "Approver"));
							  x.Claims.Add(new Claim("Custom", "Orange"));
							  x.Claims.Add(new Claim("Custom", "Blue"));
						  });
	}
}
