using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

namespace FakeAuth
{
	public class FakeAuthOptions : AuthenticationSchemeOptions
	{
		public const string DefaultAllowedHost = "localhost";
		public FakeAuthOptions()
		{
			Claims = new List<Claim>();
		}

		public IList<Claim> Claims { get; set; }

		public IEnumerable<string> AllowedHosts { get; set; } = new[] { DefaultAllowedHost };
	}
}