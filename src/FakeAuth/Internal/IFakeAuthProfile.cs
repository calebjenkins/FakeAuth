using System;

namespace developingux.FakeAuth.Internal
{
	public interface IFakeAuthProfile
	{
		Action<FakeAuthOptions> OptionBuilder();
	}
}
