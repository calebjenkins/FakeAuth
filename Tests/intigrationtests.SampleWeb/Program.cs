using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using FakeAuth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BuildAuth(builder);

builder.Services.AddAuthorization(options =>
{
		// By default, all incoming requests will be authorized according to the default policy.
		options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddRazorPages()
	 .AddMicrosoftIdentityUI();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//For(typeof(ILogger<>)).Use(typeof(Logger<>));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

public partial class Program
{

	// Added so we can override this in integration tests
	// Also added to csproj file: <InternalsVisibleTo Include="FakeAuth.IntegrationTests" />
	private static Action<WebApplicationBuilder> _buildAuth = (builder) =>
			 {
				 builder.Services.UseFakeAuth();
			 //builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
			 // .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

		 };
	public static Action<WebApplicationBuilder> BuildAuth
	{
		get => _buildAuth;
		set => _buildAuth = value;
	}
}