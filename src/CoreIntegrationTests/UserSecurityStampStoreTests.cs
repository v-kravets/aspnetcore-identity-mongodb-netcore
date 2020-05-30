namespace IntegrationTests
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class UserSecurityStampStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task Create_NewUser_HasSecurityStamp()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};

			await manager.CreateAsync(user);

			var savedUser = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single();
			Assert.NotNull(savedUser.SecurityStamp);
		}

		[Test]
		public async Task GetSecurityStamp_NewUser_ReturnsStamp()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			var stamp = await manager.GetSecurityStampAsync(user);

			Assert.NotNull(stamp);
		}
	}
}