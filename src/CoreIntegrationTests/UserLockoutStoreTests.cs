namespace IntegrationTests
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using Microsoft.Extensions.DependencyInjection;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class UserLockoutStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public async Task AccessFailed_IncrementsAccessFailedCount()
		{
			var manager = GetUserManagerWithThreeMaxAccessAttempts();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			await manager.AccessFailedAsync(user);

			Assert.AreEqual(await manager.GetAccessFailedCountAsync(user), 1);
		}

		private UserManager<IdentityUser> GetUserManagerWithThreeMaxAccessAttempts()
		{
			return CreateServiceProvider<IdentityUser, IdentityRole>(options => options.Lockout.MaxFailedAccessAttempts = 3)
				.GetService<UserManager<IdentityUser>>();
		}

		[Test]
		public void IncrementAccessFailedCount_ReturnsNewCount()
		{
			var store = new UserStore<IdentityUser>(null);
			var user = new IdentityUser {UserName = "bob"};

			var count = store.IncrementAccessFailedCountAsync(user, default(CancellationToken));

			Assert.AreEqual(count.Result, 1);
		}

		[Test]
		public async Task ResetAccessFailed_AfterAnAccessFailed_SetsToZero()
		{
			var manager = GetUserManagerWithThreeMaxAccessAttempts();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);
			await manager.AccessFailedAsync(user);

			await manager.ResetAccessFailedCountAsync(user);

			Assert.AreEqual(await manager.GetAccessFailedCountAsync(user), 0);
		}

		[Test]
		public async Task AccessFailed_NotOverMaxFailures_NoLockoutEndDate()
		{
			var manager = GetUserManagerWithThreeMaxAccessAttempts();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			await manager.AccessFailedAsync(user);

			Assert.Null(await manager.GetLockoutEndDateAsync(user));
		}

		[Test]
		public async Task AccessFailed_ExceedsMaxFailedAccessAttempts_LocksAccount()
		{
			var manager = CreateServiceProvider<IdentityUser, IdentityRole>(options =>
				{
					options.Lockout.MaxFailedAccessAttempts = 0;
					options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
				})
				.GetService<UserManager<IdentityUser>>();

			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			await manager.AccessFailedAsync(user);

			var lockoutEndDate = await manager.GetLockoutEndDateAsync(user);
			Assert.Greater(lockoutEndDate?.Subtract(DateTime.UtcNow).TotalHours, 0.9);
			Assert.Less(lockoutEndDate?.Subtract(DateTime.UtcNow).TotalHours, 1.1);
		}

		[Test]
		public async Task SetLockoutEnabled()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			await manager.CreateAsync(user);

			await manager.SetLockoutEnabledAsync(user, true);
			Assert.True(await manager.GetLockoutEnabledAsync(user));

			await manager.SetLockoutEnabledAsync(user, false);
			Assert.False(await manager.GetLockoutEnabledAsync(user));
		}
	}
}