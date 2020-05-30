namespace IntegrationTests
{
	using System.Linq;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using NUnit.Framework;
	using Tests;

	[TestFixture]
	public class UserClaimStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task Create_NewUser_HasNoClaims()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			var claims = await manager.GetClaimsAsync(user);

			Assert.IsEmpty(claims.ToList());
		}

		[Test]
		public async Task AddClaim_ReturnsClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			await manager.AddClaimAsync(user, new Claim("type", "value"));

			var claim = (await manager.GetClaimsAsync(user)).Single();
			Assert.AreEqual(claim.Type, "type");
			Assert.AreEqual(claim.Value, "value");
		}

		[Test]
		public async Task RemoveClaim_RemovesExistingClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			await manager.AddClaimAsync(user, new Claim("type", "value"));

			await manager.RemoveClaimAsync(user, new Claim("type", "value"));

			Assert.IsEmpty((await manager.GetClaimsAsync(user)).ToList());
		}

		[Test]
		public async Task RemoveClaim_DifferentType_DoesNotRemoveClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			await manager.AddClaimAsync(user, new Claim("type", "value"));

			await manager.RemoveClaimAsync(user, new Claim("otherType", "value"));

			Assert.IsNotEmpty((await manager.GetClaimsAsync(user)).ToList());
		}

		[Test]
		public async Task RemoveClaim_DifferentValue_DoesNotRemoveClaim()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			await manager.AddClaimAsync(user, new Claim("type", "value"));

			await manager.RemoveClaimAsync(user, new Claim("type", "otherValue"));

			Assert.IsNotEmpty((await manager.GetClaimsAsync(user)).ToList());
		}

		[Test]
		public async Task ReplaceClaim_Replaces()
		{
			// note: unit tests cover behavior of ReplaceClaim method on IdentityUser
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			var existingClaim = new Claim("type", "value");
			await manager.AddClaimAsync(user, existingClaim);
			var newClaim = new Claim("newType", "newValue");

			await manager.ReplaceClaimAsync(user, existingClaim, newClaim);

			user.ExpectOnlyHasThisClaim(newClaim);
		}

		[Test]
		public async Task GetUsersForClaim()
		{
			var userWithClaim = new IdentityUser
			{
				UserName = "with"
			};
			var userWithout = new IdentityUser();
			var manager = GetUserManager();
			await manager.CreateAsync(userWithClaim);
			await manager.CreateAsync(userWithout);
			var claim = new Claim("sameType", "sameValue");
			await manager.AddClaimAsync(userWithClaim, claim);

			var matchedUsers = await manager.GetUsersForClaimAsync(claim);

			Assert.AreEqual(matchedUsers.Count, 1);
			Assert.AreEqual(matchedUsers.Single().UserName, "with");

			var matchesForWrongType = await manager.GetUsersForClaimAsync(new Claim("wrongType", "sameValue"));
			Assert.IsEmpty(matchesForWrongType);

			var matchesForWrongValue = await manager.GetUsersForClaimAsync(new Claim("sameType", "wrongValue"));
			Assert.IsEmpty(matchesForWrongValue);
		}
	}
}