using System;
using System.Security.Claims;
using static Developingux.FakeAuth.Internal.FakeAuthConst;

namespace Developingux.FakeAuth.Internal
{
	public class DefaultProfile : IFakeAuthProfile
	{
		public Action<FakeAuthOptions> OptionBuilder()
		{
			return new Action<FakeAuthOptions>(x =>
								{
									x.Claims.Add(new Claim(ClaimTypes.Name, FakeUser.Name));
									x.Claims.Add(new Claim(ClaimTypes.Email, FakeUser.Email));
								});
		}
	}
}
