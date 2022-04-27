using FakeAuth.Profiles;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace FakeAuth.Testing
{
	public static class HttpClientExtensions
	{
		public static void SetFakeAuthClaims<TProfile>(this HttpClient client) where TProfile : IFakeAuthProfile, new()
		{
			TProfile profile = new TProfile();
			var claims = profile.GetClaims().ToArray();
			client.SetFakeAuthClaims(claims);
		}

		public static void SetFakeAuthClaims(this HttpClient client, params Claim[] claims)
		{
			client.DefaultRequestHeaders.Remove(FakeAuthDefaults.ClaimsHeaderName);

			foreach (var c in claims)
			{
				client.DefaultRequestHeaders.Add(FakeAuthDefaults.ClaimsHeaderName, $"{c.Type},{c.Value}");
			}
		}
	}
}