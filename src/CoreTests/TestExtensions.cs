namespace Tests
{
	using System.Linq;
	using System.Security.Claims;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using NUnit.Framework;

	public static class TestExtensions
	{
		public static void ExpectOnlyHasThisClaim(this IdentityUser user, Claim expectedClaim)
		{
			Assert.AreEqual(user.Claims.Count, 1);
			var actualClaim = user.Claims.Single();
			Assert.AreEqual(actualClaim.Type, expectedClaim.Type);
			Assert.AreEqual(actualClaim.Value,expectedClaim.Value);
		}
	}
}