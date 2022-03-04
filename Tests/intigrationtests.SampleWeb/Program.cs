using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using FakeAuth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
	.AddFakeAuth();

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

// here to ensure visibility of Program class to tests
public partial class Program
{
}