using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;

namespace Developingux.FakeAuth
{
	public class FakeAuthOptions : AuthenticationSchemeOptions
	{
		public FakeAuthOptions()
		{
			Claims = new List<Claim>();
		}

		public List<Claim> Claims { get; set; }
	}
}
