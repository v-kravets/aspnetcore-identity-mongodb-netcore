namespace IntegrationTests
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class RoleStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task Create_NewRole_Saves()
		{
			var roleName = "admin";
			var role = new IdentityRole(roleName);
			var manager = GetRoleManager();

			await manager.CreateAsync(role);

			var savedRole = Roles.FindSync<IdentityRole>(MongoDB.Driver.FilterDefinition<IdentityRole>.Empty).FirstOrDefault();
			Assert.AreEqual(savedRole.Name, roleName);
			Assert.AreEqual(savedRole.NormalizedName, "ADMIN");
		}

		[Test]
		public async Task FindByName_SavedRole_ReturnsRole()
		{
			var roleName = "name";
			var role = new IdentityRole {Name = roleName};
			var manager = GetRoleManager();
			await manager.CreateAsync(role);

			// note: also tests normalization as FindByName now uses normalization
			var foundRole = await manager.FindByNameAsync(roleName);

			Assert.NotNull(foundRole);
			Assert.AreEqual(foundRole.Name, roleName);
		}

		[Test]
		public async Task FindById_SavedRole_ReturnsRole()
		{
			var roleId = ObjectId.GenerateNewId().ToString();
			var role = new IdentityRole {Name = "name"};
			role.Id = roleId;
			var manager = GetRoleManager();
			await manager.CreateAsync(role);

			var foundRole = await manager.FindByIdAsync(roleId);

			Assert.NotNull(foundRole);
			Assert.AreEqual(foundRole.Id, roleId);
		}

		[Test]
		public async Task Delete_ExistingRole_Removes()
		{
			var role = new IdentityRole {Name = "name"};
			var manager = GetRoleManager();
			await manager.CreateAsync(role);
			Assert.IsNotEmpty(Roles.FindSync<IdentityRole>(MongoDB.Driver.FilterDefinition<IdentityRole>.Empty).ToList());

			await manager.DeleteAsync(role);

			Assert.IsEmpty(Roles.FindSync<IdentityRole>(MongoDB.Driver.FilterDefinition<IdentityRole>.Empty).ToList());
		}

		[Test]
		public async Task Update_ExistingRole_Updates()
		{
			var role = new IdentityRole {Name = "name"};
			var manager = GetRoleManager();
			await manager.CreateAsync(role);
			var savedRole = await manager.FindByIdAsync(role.Id);
			savedRole.Name = "newname";

			await manager.UpdateAsync(savedRole);

			var changedRole = Roles.FindSync<IdentityRole>(MongoDB.Driver.FilterDefinition<IdentityRole>.Empty).FirstOrDefault();
			Assert.NotNull(changedRole);
			Assert.AreEqual(changedRole.Name, "newname");
		}

		[Test]
		public async Task SimpleAccessorsAndGetters()
		{
			var role = new IdentityRole
			{
				Name = "name"
			};
			var manager = GetRoleManager();
			await manager.CreateAsync(role);

			Assert.AreEqual(await manager.GetRoleIdAsync(role), role.Id);
			Assert.AreEqual(await manager.GetRoleNameAsync(role), "name");

			await manager.SetRoleNameAsync(role, "newName");
			Assert.AreEqual(await manager.GetRoleNameAsync(role), "newName");
		}
	}
}