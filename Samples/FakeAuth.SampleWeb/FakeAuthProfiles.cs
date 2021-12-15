using System.Security.Claims;
using System;
using FakeAuth.Profiles;

namespace FakeAuth.SampleWeb
{
	public class FakeJoe : IFakeAuthProfile
	{
		public Action<FakeAuthOptions> OptionBuilder()
		{
			return new Action<FakeAuthOptions>(options =>
			{
				options.Claims.Add(new Claim(ClaimTypes.Name, "Fake Joe"));
				options.Claims.Add(new Claim(ClaimTypes.Role, "Expense_Approver"));
				options.Claims.Add(new Claim("Approval_Limit", "55.00"));
				options.Claims.Add(new Claim("Approval_Currency", "USD"));
				options.Claims.Add(new Claim("Preffered_Location", "Sunny Hammock"));
			});
		}
	}

	public class FakeSally : IFakeAuthProfile
	{
		public Action<FakeAuthOptions> OptionBuilder()
		{
			return new Action<FakeAuthOptions>(options =>
			{
				options.Claims.Add(new Claim(ClaimTypes.Name, "Fake Sally"));
				options.Claims.Add(new Claim(ClaimTypes.Role, "Expense_Approver"));
				options.Claims.Add(new Claim(ClaimTypes.Role, "Buyer"));
				options.Claims.Add(new Claim("Approval_Limit", "1000.00"));
				options.Claims.Add(new Claim("Approval_Currency", "USD"));
				options.Claims.Add(new Claim("Preffered_Location", "Ocean View"));
			});
		}
	}
}
