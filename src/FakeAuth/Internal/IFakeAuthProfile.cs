using System;

namespace Developingux.FakeAuth.Internal
{
	public interface IFakeAuthProfile
	{
		Action<FakeAuthOptions> OptionBuilder();
	}
}
