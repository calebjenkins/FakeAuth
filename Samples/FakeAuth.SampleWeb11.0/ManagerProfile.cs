using System.Security.Claims;
using FakeAuth;
using FakeAuth.Profiles;

namespace FakeAuthSample;

public sealed class ManagerProfile : IFakeAuthProfile
{
    public Action<FakeAuthOptions> OptionBuilder() => options =>
    {
        options.Claims.Add(new Claim(ClaimTypes.Name, "Manager User"));
        options.Claims.Add(new Claim(ClaimTypes.Email, "manager@example.test"));
        options.Claims.Add(new Claim(ClaimTypes.Role, "Manager"));
    };
}

