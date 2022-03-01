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
			if (!CurrentUri.ToUpper().Contains("://LOCALHOST"))
			{
				_logger.LogError("Library only intended for localhost developement");
				return AuthenticateResult.Fail("FakeAuth can only be used for localhost developement. Please impliment another OAuth solution for other scenarios");
			}

			var claims = Options.Claims;
			if (Request.Headers.ContainsKey(FakeAuthDefaults.ClaimsHeaderName))
			{
				var headerVal = Request.Headers[FakeAuthDefaults.ClaimsHeaderName][0];
				using var stream = new MemoryStream(Convert.FromBase64String(headerVal));
				using var reader = new BinaryReader(stream);

				claims = new List<Claim>();
				while (stream.Position < stream.Length)
				{
					claims.Add(new Claim(reader));
				}
			}

			var identity = new ClaimsIdentity(claims, Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);
			return AuthenticateResult.Success(ticket);
		}
	}
}