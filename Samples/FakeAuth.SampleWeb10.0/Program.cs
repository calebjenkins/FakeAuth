using System.Security.Claims;
using FakeAuth;
using FakeAuthSample;

var builder = WebApplication.CreateBuilder(args);

// Choose one of the three registration styles with --profile.
var authentication = builder.Services.AddAuthentication();
switch (builder.Configuration["profile"] ?? "default")
{
    case "default":
        authentication.AddFakeAuth();
        break;
    case "manager":
        authentication.AddFakeAuth<ManagerProfile>();
        break;
    case "inline":
        authentication.AddFakeAuth(options =>
        {
            options.Claims.Add(new Claim(ClaimTypes.Name, "Inline User"));
            options.Claims.Add(new Claim(ClaimTypes.Email, "inline@example.test"));
            options.Claims.Add(new Claim(ClaimTypes.Role, "Manager"));
        });
        break;
    default:
        throw new ArgumentException("Use --profile default, manager, or inline.");
}

builder.Services.AddAuthorization();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    description = "FakeAuth development sample",
    endpoints = new[] { "/api/claims", "/api/manager" },
    profile = builder.Configuration["profile"] ?? "default"
}));
app.MapGet("/api/claims", (ClaimsPrincipal user) =>
    user.Claims.Select(claim => new { claim.Type, claim.Value }))
    .RequireAuthorization();
app.MapGet("/api/manager", (ClaimsPrincipal user) =>
    Results.Ok(new { message = "Manager access granted", name = user.Identity?.Name }))
    .RequireAuthorization(policy => policy.RequireRole("Manager"));

app.Run();

