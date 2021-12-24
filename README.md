# FakeAuth

[![.github/workflows/dev-ci.yml](https://github.com/calebjenkins/FakeAuth/actions/workflows/dev-ci.yml/badge.svg?branch=Develop)](https://github.com/calebjenkins/FakeAuth/actions/workflows/dev-ci.yml)
[![.github/workflows/main-ci-test-publish.yml](https://github.com/calebjenkins/FakeAuth/actions/workflows/main-ci-test-publish.yml/badge.svg?branch=main)](https://github.com/calebjenkins/FakeAuth/actions/workflows/main-ci-test-publish.yml)
[![NuGet](https://img.shields.io/nuget/dt/fakeauth.svg)](https://www.nuget.org/packages/fakeauth) 
[![NuGet](https://img.shields.io/nuget/vpre/fakeauth.svg)](https://www.nuget.org/packages/fakeauth)

a .NET Core library to make developing offline for OAuth and OIDC much easier. 

Developing with OAuth or OIDC - takes about 30 minutes of set up work just to get going; with FakeAuth, it's one line of code.  

Supports custom Claims and Profiles that can be swapped in during developement of your application. 

Examples in the [Samples Folder](https://github.com/calebjenkins/FakeAuth/tree/main/Samples).

### Installing FakeAuth

You should install [FakeAuth with NuGet](https://www.nuget.org/packages/FakeAuth):

    Install-Package FakeAuth
    
Or via the .NET Core command line interface:

    dotnet add package FakeAuth

Either commands, from Package Manager Console or .NET Core CLI, will download and install FakeAuth and all required dependencies.

### Using FakeAuth

In an ASP.NET Core Application, you can configure FakeAuth in the Startup Class:

    services.UseFakeAuth();

That will give you a default profile. In fact, the above is exactly the same as doing this:

    services.UseFakeAuth<DefaultProfile>();

You can created custom profiles by implementing the interface [IFakeAuthProfile](https://github.com/calebjenkins/FakeAuth/blob/main/src/FakeAuth/Profiles/IFakeAuthProfile.cs)

Or you can inline your custom claims directly:

    services.UseFakeAuth((options) =>
    {
		options.Claims.Add(new Claim(ClaimTypes.Name, "Fake User"));
		options.Claims.Add(new Claim(ClaimTypes.Role, "Expense_Approver"));
		options.Claims.Add(new Claim("Approval_Limit", "25.00"));
		options.Claims.Add(new Claim("Approval_Currency", "USD"));
		options.Claims.Add(new Claim("Preffered_Location", "Disney Island"));
	});

See more of these examples in the [SampleWeb application](https://github.com/calebjenkins/FakeAuth/tree/main/Samples/FakeAuth.SampleWeb)

### .NET 6

In .NET 6 you are no longer required to use a StartUp class. You can still use FakeAuth directly in the [Program class](https://github.com/calebjenkins/FakeAuth/blob/main/Samples/nuget.SampleWeb6.0/Program.cs):

    builder.Services.UseFakeAuth();

### Use Cases - for OAth/Claims based .NET Core applications

- To get started building your application as quickly as possible.
- For POCs that you want to try out without registering your application in an Identity Provider.
- For running and developing locally without internet access.
- For Demo based applications that you want people to download and run - without needing to set up a production identity service first, or without sharing your application id/client secret information. 

### Not for - FakeAuth can not be used in production
- Do not use FakeAuth in a production enviroment
- FakeAUth will only work on http://localhost/ - it's intededed to be a developent tool.
- You will want to transition to an actual OAth / Claims provider before you go to Production



