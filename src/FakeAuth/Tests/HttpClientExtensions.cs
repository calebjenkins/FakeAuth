using System;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using FakeAuth.Internal;

namespace FakeAuth.Tests
{
	public static class HttpClientExtensions
	{
		public static void SetFakeAuthClaims(this HttpClient client, params Claim[] claims)
		{
			client.DefaultRequestHeaders.Remove(FakeAuthConst.ClaimsHeaderName);

			using var stream = new MemoryStream();
			using var writer = new BinaryWriter(stream);

			foreach (var c in claims)
			{
				c.WriteTo(writer);
			}

			var headerValue = Convert.ToBase64String(stream.ToArray());

			client.DefaultRequestHeaders.Add(FakeAuthConst.ClaimsHeaderName, headerValue);
		}
	}
}