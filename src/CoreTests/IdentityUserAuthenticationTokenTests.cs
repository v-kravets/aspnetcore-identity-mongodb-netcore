namespace CoreTests
{
	using Microsoft.AspNetCore.Identity.MongoDB;
	using NUnit.Framework;

	public class IdentityUserAuthenticationTokenTests
	{
		[Test]
		public void GetToken_NoTokens_ReturnsNull()
		{
			var user = new IdentityUser();

			var value = user.GetTokenValue("loginProvider", "tokenName");

			Assert.Null(value);
		}

		[Test]
		public void GetToken_WithToken_ReturnsValueIfProviderAndNameMatch()
		{
			var user = new IdentityUser();
			user.SetToken("loginProvider", "tokenName", "tokenValue");

			Assert.AreEqual(user.GetTokenValue("loginProvider", "tokenName"), "tokenValue");

			Assert.Null(user.GetTokenValue("wrongProvider", "tokenName"));

			Assert.Null(user.GetTokenValue("loginProvider", "wrongName"));
		}

		[Test]
		public void RemoveToken_OnlyRemovesIfNameAndProviderMatch()
		{
			var user = new IdentityUser();
			user.SetToken("loginProvider", "tokenName", "tokenValue");

			user.RemoveToken("wrongProvider", "tokenName");
			Assert.AreEqual(user.GetTokenValue("loginProvider", "tokenName"), "tokenValue");

			user.RemoveToken("loginProvider", "wrongName");
			Assert.AreEqual(user.GetTokenValue("loginProvider", "tokenName"),"tokenValue");

			user.RemoveToken("loginProvider", "tokenName");
			Assert.Null(user.GetTokenValue("loginProvider", "tokenName"));
		}

		[Test]
		public void SetToken_ReplacesValue()
		{
			var user = new IdentityUser();
			user.SetToken("loginProvider", "tokenName", "tokenValue");

			user.SetToken("loginProvider", "tokenName", "updatedValue");

			Assert.AreEqual(user.Tokens.Count, 1);
			Assert.AreEqual(user.GetTokenValue("loginProvider", "tokenName"),"updatedValue");
		}
	}
}