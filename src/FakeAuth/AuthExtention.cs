using FakeAuth.Internal;
using FakeAuth.Profiles;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FakeAuth
{
	public static class AuthExtension
	{
		public static void UseFakeAuth(this IServiceCollection services)
		{
			services.UseFakeAuth<DefaultProfile>();
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
