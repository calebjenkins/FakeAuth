using System.Collections.Generic;
using System.Security.Claims;

namespace FakeAuth.Profiles
{
	public static class FakeAuthProfileExtension
	{
		public static IList<string> GetClaimKeys(this IFakeAuthProfile profile)
		{

			List<string> keys = new List<string>();
			var claims = (List < Claim >) profile.GetClaims();
			
			claims.ForEach((c) =>
			{
				if (!keys.Contains(c.Type))
				{
					keys.Add(c.Type);
				}
			});

			return keys;
		}

		public static IList<Claim> GetClaims(this IFakeAuthProfile profile)
		{
			var builder = profile.OptionBuilder();

			FakeAuthOptions options = new FakeAuthOptions();
			profile.OptionBuilder().Invoke(options);
			return options.Claims;
		}
	}
}