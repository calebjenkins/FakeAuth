using FakeAuth.Profiles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FakeAuth
{
	public static class AuthExtension
	{
		public static AuthenticationBuilder AddFakeAuth(this AuthenticationBuilder authbuilder)
		{
			return AddFakeAuth<DefaultProfile>(authbuilder);
		}
		public static AuthenticationBuilder AddFakeAuth<TProfile>(this AuthenticationBuilder authbuilder) where TProfile : IFakeAuthProfile, new()
		{
			IFakeAuthProfile profile = new TProfile();
			Action<FakeAuthOptions> options = profile.OptionBuilder();
			
			return AddFakeAuth(authbuilder, options);
		}

		public static AuthenticationBuilder AddFakeAuth(this AuthenticationBuilder authbuilder, Action<FakeAuthOptions> options)
		{
			authbuilder.Services.AddAuthentication(FakeAuthDefaults.SchemaName)
				.AddScheme<FakeAuthOptions, FakeAuthHandler>(FakeAuthDefaults.SchemaName, null);

			authbuilder.Services.Configure<FakeAuthOptions>(FakeAuthDefaults.SchemaName, options);

			return authbuilder;
		}

		[Obsolete("Please use AddAuthentication().AddFakeAuth() instead") ]
		public static void UseFakeAuth(this IServiceCollection services)
		{
			services.UseFakeAuth<DefaultProfile>();
		}

		[Obsolete("Please use AddAuthentication().AddFakeAuth() instead")]
		public static void UseFakeAuth<TProfile>(this IServiceCollection services) where TProfile : IFakeAuthProfile, new()
		{
			IFakeAuthProfile profile = new TProfile();
			Action<FakeAuthOptions> options = profile.OptionBuilder();
			UseFakeAuth(services, options);
		}

		[Obsolete("Please use AddAuthentication().AddFakeAuth() instead")]
		public static void UseFakeAuth(this IServiceCollection services, Action<FakeAuthOptions> options)
		{
			services.AddAuthentication(FakeAuthDefaults.SchemaName)
			.AddScheme<FakeAuthOptions, FakeAuthHandler>(FakeAuthDefaults.SchemaName, null);

			services.Configure<FakeAuthOptions>(FakeAuthDefaults.SchemaName, options);
		}
	}
}