using Developingux.FakeAuth.Internal;
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

		public string Realm = FakeAuthConst.SchemaName;
		public List<Claim> Claims { get; set; }
	}
}
