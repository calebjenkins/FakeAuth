using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FakeAuth
{
	public class FakeAuthHandler : AuthenticationHandler<FakeAuthOptions>
	{
		private readonly ILogger<FakeAuthHandler> _logger;

		public FakeAuthHandler(IOptionsMonitor<FakeAuthOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock
		)
		: base(options, logger, encoder, clock)
		{
			_logger = logger.CreateLogger<FakeAuthHandler>();
			_logger.LogDebug("Created the FakeAuth authentication handler.");
		}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			var host = Context.Request.Host.Host;
			if (host.ToUpper() != Options.AllowedHost.ToUpper())
			{
				_logger.LogError("Failing authentication due to unexpected host {Host} when allowed host is {AllowedHost}", host, Options.AllowedHost);
				return AuthenticateResult.Fail($"FakeAuth fails all requests that do not match {Options.AllowedHost}; got host {host}.");
			}

			var claims = Options.Claims;
			if (Request.Headers.ContainsKey(FakeAuthDefaults.ClaimsHeaderName))
			{
				var claimValues = Request.Headers[FakeAuthDefaults.ClaimsHeaderName];

				claims = new List<Claim>();
				foreach(var c in claimValues)
				{
					var parts = c.Split(",");
					claims.Add(new Claim(parts[0], parts[1]));
				}
			}

			var identity = new ClaimsIdentity(claims, Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);
			return AuthenticateResult.Success(ticket);
		}
	}
}