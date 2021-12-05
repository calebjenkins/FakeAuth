using System;
using System.Security.Claims;
using static FakeAuth.Internal.FakeAuthConst;

namespace FakeAuth.Profiles
{
	public class AzureProfile : IFakeAuthProfile
	{
		public Action<FakeAuthOptions> OptionBuilder()
		{
			return new Action<FakeAuthOptions>(x =>
			{
				x.Claims.Add(azureClaim("preferred_username", FakeUser.Email));
				x.Claims.Add(azureClaim("name", FakeUser.Name));
				x.Claims.Add(azureClaim(ClaimTypes.NameIdentifier, "FAKE"));
				x.Claims.Add(azureClaim("aio", "fAkezxy"));
				x.Claims.Add(azureClaim("http://schemas.microsoft.com/identity/claims/objectidentifier", "FAKE"));
				x.Claims.Add(azureClaim("http://schemas.microsoft.com/identity/claims/tenantid", "FAKE"));
			});
		}

		private static Claim azureClaim(string key, string value)
		{
			var issuer = "https://login.microsoftonline.com/FAKE-GUID/v2.0";
			var xmlValueType = "http://www.w3.org/2001/XMLSchema#string";

			return new Claim(key, value, xmlValueType, issuer, issuer);
		}
	}
}
