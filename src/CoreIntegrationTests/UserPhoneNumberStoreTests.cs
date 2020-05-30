namespace IntegrationTests
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class UserPhoneNumberStoreTests : UserIntegrationTestsBase
	{
		private const string PhoneNumber = "1234567890";

		[Test]
		public async Task SetPhoneNumber_StoresPhoneNumber()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);

			await manager.SetPhoneNumberAsync(user, PhoneNumber);

			Assert.AreEqual(await manager.GetPhoneNumberAsync(user), PhoneNumber);
		}

		[Test]
		public async Task ConfirmPhoneNumber_StoresPhoneNumberConfirmed()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			var token = await manager.GenerateChangePhoneNumberTokenAsync(user, PhoneNumber);

			await manager.ChangePhoneNumberAsync(user, PhoneNumber, token);

			Assert.True(await manager.IsPhoneNumberConfirmedAsync(user));
		}

		[Test]
		public async Task ChangePhoneNumber_OriginalPhoneNumberWasConfirmed_NotPhoneNumberConfirmed()
		{
			var user = new IdentityUser {UserName = "bob"};
			var manager = GetUserManager();
			await manager.CreateAsync(user);
			var token = await manager.GenerateChangePhoneNumberTokenAsync(user, PhoneNumber);
			await manager.ChangePhoneNumberAsync(user, PhoneNumber, token);

			await manager.SetPhoneNumberAsync(user, PhoneNumber);

			Assert.False(await manager.IsPhoneNumberConfirmedAsync(user));
		}
	}
}