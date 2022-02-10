using System;

namespace FakeAuth.Profiles
{
	public interface IFakeAuthProfile
	{
		Action<FakeAuthOptions> OptionBuilder();
	}
}