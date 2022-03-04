using FakeAuth.Profiles;
using System;
using System.Security.Claims;

namespace FakeAuth.IntegrationTests;

public class ManagerJoeProfile : IFakeAuthProfile
{
	public Action<FakeAuthOptions> OptionBuilder()
	{
		return new Action<FakeAuthOptions>(x =>
		{
			x.Claims.Add(new Claim(ClaimTypes.Name, "Joe Manager"));
			x.Claims.Add(new Claim(ClaimTypes.Role, "Manager"));
		});
	}
}

public class NonManagerJoeProfile : IFakeAuthProfile
{
	public Action<FakeAuthOptions> OptionBuilder()
	{
		return new Action<FakeAuthOptions>(x =>
		{
			x.Claims.Add(new Claim(ClaimTypes.Name, "Joe Manager"));
			x.Claims.Add(new Claim(ClaimTypes.Role, "Employee"));
		});
	}
}
