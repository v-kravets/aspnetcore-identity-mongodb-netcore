namespace IntegrationTests
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class UserStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task Create_NewUser_Saves()
		{
			var userName = "name";
			var user = new IdentityUser {UserName = userName};
			var manager = GetUserManager();

			await manager.CreateAsync(user);

			var savedUser = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single();
			Assert.AreEqual(savedUser.UserName, user.UserName);
		}

		[Test]
		public async Task FindByName_SavedUser_ReturnsUser()
		{
			var userName = "name";
			var user = new IdentityUser {UserName = userName};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			var foundUser = await manager.FindByNameAsync(userName);

			Assert.NotNull(foundUser);
			Assert.AreEqual(foundUser.UserName, userName);
		}

		[Test]
		public async Task FindByName_NoUser_ReturnsNull()
		{
			var manager = GetUserManager();

			var foundUser = await manager.FindByNameAsync("nouserbyname");

			Assert.Null(foundUser);
		}

		[Test]
		public async Task FindById_SavedUser_ReturnsUser()
		{
			var userId = ObjectId.GenerateNewId().ToString();
			var user = new IdentityUser {UserName = "name"};
			user.Id = userId;
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			var foundUser = await manager.FindByIdAsync(userId);

			Assert.NotNull(foundUser);
			Assert.AreEqual(foundUser.Id, userId);
		}

		[Test]
		public async Task FindById_NoUser_ReturnsNull()
		{
			var manager = GetUserManager();

			var foundUser = await manager.FindByIdAsync(ObjectId.GenerateNewId().ToString());

			Assert.Null(foundUser);
		}

		[Test]
		public async Task FindById_IdIsNotAnObjectId_ReturnsNull()
		{
			var manager = GetUserManager();

			var foundUser = await manager.FindByIdAsync("notanobjectid");

			Assert.Null(foundUser);
		}

		[Test]
		public async Task Delete_ExistingUser_Removes()
		{
			var user = new IdentityUser {UserName = "name"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			Assert.IsNotEmpty(Users.FindSync(FilterDefinition<IdentityUser>.Empty).ToList());

			await manager.DeleteAsync(user);

			Assert.IsEmpty(Users.FindSync(FilterDefinition<IdentityUser>.Empty).ToList());
		}

		[Test]
		public async Task Update_ExistingUser_Updates()
		{
			var user = new IdentityUser {UserName = "name"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			var savedUser = await manager.FindByIdAsync(user.Id);
			savedUser.UserName = "newname";

			await manager.UpdateAsync(savedUser);

			var changedUser = Users.FindSync(FilterDefinition<IdentityUser>.Empty).Single();
			Assert.NotNull(changedUser);
			Assert.AreEqual(changedUser.UserName, "newname");
		}

		[Test]
		public async Task SimpleAccessorsAndGetters()
		{
			var user = new IdentityUser
			{
				UserName = "username"
			};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			Assert.AreEqual(await manager.GetUserIdAsync(user), user.Id);
			Assert.AreEqual(await manager.GetUserNameAsync(user), "username");

			await manager.SetUserNameAsync(user, "newUserName");
			Assert.AreEqual(await manager.GetUserNameAsync(user), "newUserName");
		}
	}
}