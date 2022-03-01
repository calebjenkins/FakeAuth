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
			authbuilder.Services.UseFakeAuth();
			return authbuilder;
		}
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
			services.AddAuthentication(FakeAuthDefaults.SchemaName)
			.AddScheme<FakeAuthOptions, FakeAuthHandler>(FakeAuthDefaults.SchemaName, null);

			services.Configure<FakeAuthOptions>(FakeAuthDefaults.SchemaName, options);
		}
	}
}