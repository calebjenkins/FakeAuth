using developingux.FakeAuth.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace developingux.FakeAuth
{
	public static class AuthExtension
	{
		public static void UseFakeAuth(this IServiceCollection services, FakeAuthProfile profile = FakeAuthProfile.DEFAULT)
		{
			switch (profile)
			{
				case FakeAuthProfile.DEFAULT:
					{
						UseFakeAuth<DefaultProfile>(services);
						break;
					}
				case FakeAuthProfile.AZURE_AD:
					{
						UseFakeAuth<AzureProfile>(services);
						break;
					}
			}
		}

		public static void UseFakeAuth<TProfile>(this IServiceCollection services) where TProfile : IFakeAuthProfile, new()
		{
			IFakeAuthProfile profile = new TProfile();
			Action<FakeAuthOptions> options = profile.OptionBuilder();
			UseFakeAuth(services, options);
		}

		public static void UseFakeAuth(this IServiceCollection services, Action<FakeAuthOptions> options)
		{
			services.AddAuthentication(FakeAuthConst.SchemaName)
			.AddScheme<FakeAuthOptions, FakeAuthHandler>(FakeAuthConst.SchemaName, null);

			services.Configure<FakeAuthOptions>(FakeAuthConst.SchemaName, options);
		}
	}
}
