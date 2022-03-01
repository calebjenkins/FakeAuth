using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FakeAuth.Profiles;

namespace FakeAuth.SampleWeb
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Azure AD Auth:
			//services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
			//	 .AddAzureAD(options => Configuration.Bind("AzureAd", options));

			// // Package: FakeAuth with default profile
			//services.UseFakeAuth();

			// // FakeAuth with pre-defined profile
			// services.UseFakeAuth<AzureProfile>();
			// services.UseFakeAuth<DefaultProfile>();

			// // FakeAuth with Custom Profile
			// services.UseFakeAuth<FakeJoe>();
			// services.UseFakeAuth<FakeSally>();

			// // FakeAuth with in-line custom options
			//services.UseFakeAuth((options) =>
			//{
			//	options.Claims.Add(new Claim(ClaimTypes.Name, "Fake User"));
			//	options.Claims.Add(new Claim(ClaimTypes.Role, "Expense_Approver"));
			//	options.Claims.Add(new Claim("Approval_Limit", "25.00"));
			//	options.Claims.Add(new Claim("Approval_Currency", "USD"));
			//	options.Claims.Add(new Claim("Preffered_Location", "Disney Island"));
			//});
			services.AddAuthentication(FakeAuthDefaults.SchemaName)
				.AddFakeAuth();

			services.AddControllersWithViews(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
						 .RequireAuthenticatedUser()
						 .Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			});
			services.AddRazorPages();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
						 name: "default",
						 pattern: "{controller=Home}/{action=Index}/{id?}");
				endpoints.MapRazorPages();
			});
		}
	}
}
