using FakeAuth.Profiles;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace FakeAuth
{
	public static class HttpClientExtensions
	{
		public static void SetFakeAuthClaimns<TProfile>(this HttpClient client) where TProfile : IFakeAuthProfile, new()
		{
			TProfile profile = new TProfile();
			var claims = profile.GetClaims().ToArray();
			SetFakeAuthClaims(client, claims);
		}

		public static void SetFakeAuthClaims(this HttpClient client, params Claim[] claims)
		{
			client.DefaultRequestHeaders.Remove(FakeAuthDefaults.ClaimsHeaderName);

			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);

			foreach (var c in claims)
			{
				c.WriteTo(writer);
			}

			var headerValue = Convert.ToBase64String(stream.ToArray());

			client.DefaultRequestHeaders.Add(FakeAuthDefaults.ClaimsHeaderName, headerValue);
		}
	}
}