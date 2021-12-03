using System;

namespace Developingux.FakeAuth.Profiles
{
	public interface IFakeAuthProfile
	{
		Action<FakeAuthOptions> OptionBuilder();
	}
}
