namespace IntegrationTests
{
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Driver;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class UserRoleStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task GetRoles_UserHasNoRoles_ReturnsNoRoles()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			var roles = await manager.GetRolesAsync(user);

			Assert.IsEmpty(roles.ToList());
		}

		[Test]
		public async Task AddRole_Adds()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			await manager.AddToRoleAsync(user, "role");

			var savedUser = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single();
			// note: addToRole now passes a normalized role name
			Assert.True(savedUser.Roles.SequenceEqual(new[] {"ROLE"}));
			Assert.True(await manager.IsInRoleAsync(user, "role"));
		}

		[Test]
		public async Task RemoveRole_Removes()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AddToRoleAsync(user, "role");

			await manager.RemoveFromRoleAsync(user, "role");

			var savedUser = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single();
			Assert.IsEmpty(savedUser.Roles.ToList());
			Assert.False(await manager.IsInRoleAsync(user, "role"));
		}

		[Test]
		public async Task GetUsersInRole_FiltersOnRole()
		{
			var roleA = "roleA";
			var roleB = "roleB";
			var userInA = new IdentityUser {UserName = "nameA"};
			var userInB = new IdentityUser {UserName = "nameB"};
			var manager = GetUserManager();
			await manager.CreateAsync(userInA);
			await manager.CreateAsync(userInB);
			await manager.AddToRoleAsync(userInA, roleA);
			await manager.AddToRoleAsync(userInB, roleB);

			var matchedUsers = await manager.GetUsersInRoleAsync("roleA");

			Assert.AreEqual(matchedUsers.Count, 1);
			Assert.AreEqual(matchedUsers.First().UserName, "nameA");
		}
	}
}