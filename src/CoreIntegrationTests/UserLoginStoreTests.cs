namespace IntegrationTests
{
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Driver;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class UserLoginStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task AddLogin_NewLogin_Adds()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key", "name");
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			await manager.AddLoginAsync(user, login);

			var savedLogin = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single().Logins.Single();
			Assert.AreEqual(savedLogin.LoginProvider, "provider");
			Assert.AreEqual(savedLogin.ProviderKey, "key");
			Assert.AreEqual(savedLogin.ProviderDisplayName, "name");
		}

		[Test]
		public async Task RemoveLogin_NewLogin_Removes()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key", "name");
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AddLoginAsync(user, login);

			await manager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);

			var savedUser = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single();
			Assert.IsEmpty(savedUser.Logins.ToList());
		}

		[Test]
		public async Task GetLogins_OneLogin_ReturnsLogin()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key", "name");
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AddLoginAsync(user, login);

			var logins = await manager.GetLoginsAsync(user);

			var savedLogin = logins.Single();
			Assert.AreEqual(savedLogin.LoginProvider, "provider");
			Assert.AreEqual(savedLogin.ProviderKey, "key");
			Assert.AreEqual(savedLogin.ProviderDisplayName, "name");
		}

		[Test]
		public async Task Find_UserWithLogin_FindsUser()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key", "name");
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AddLoginAsync(user, login);

			var findUser = await manager.FindByLoginAsync(login.LoginProvider, login.ProviderKey);

			Assert.NotNull(findUser);
		}

		[Test]
		public async Task Find_UserWithDifferentKey_DoesNotFindUser()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key", "name");
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AddLoginAsync(user, login);

			var findUser = await manager.FindByLoginAsync("provider", "otherkey");

			Assert.Null(findUser);
		}

		[Test]
		public async Task Find_UserWithDifferentProvider_DoesNotFindUser()
		{
			var manager = GetUserManager();
			var login = new UserLoginInfo("provider", "key", "name");
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AddLoginAsync(user, login);

			var findUser = await manager.FindByLoginAsync("otherprovider", "key");

			Assert.Null(findUser);
		}
	}
}