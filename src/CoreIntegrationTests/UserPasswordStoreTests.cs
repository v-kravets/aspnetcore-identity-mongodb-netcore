namespace IntegrationTests
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using Microsoft.Extensions.DependencyInjection;
	using MongoDB.Driver;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class UserPasswordStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task HasPassword_NoPassword_ReturnsFalse()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			var hasPassword = await manager.HasPasswordAsync(user);

			Assert.False(hasPassword);
		}

		[Test]
		public async Task AddPassword_NewPassword_CanFindUserByPassword()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = CreateServiceProvider<IdentityUser, IdentityRole>(options =>
				{
					options.Password.RequireDigit = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
				})
				.GetService<UserManager<IdentityUser>>();
			await manager.CreateAsync(user);

			var result = await manager.AddPasswordAsync(user, "testtest");
			Assert.True(result.Succeeded);

			var userByName = await manager.FindByNameAsync("bob");
			Assert.NotNull(userByName);
			var passwordIsValid = await manager.CheckPasswordAsync(userByName, "testtest");
			Assert.True(passwordIsValid);
		}

		[Test]
		public async Task RemovePassword_UserWithPassword_SetsPasswordNull()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			await manager.AddPasswordAsync(user, "testtest");

			await manager.RemovePasswordAsync(user);

			var savedUser = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single();
			Assert.Null(savedUser.PasswordHash);
		}
	}
}