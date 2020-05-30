namespace IntegrationTests
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using Microsoft.Extensions.DependencyInjection;
	using MongoDB.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class EnsureWeCanExtendIdentityUserTests : UserIntegrationTestsBase
	{
		private UserManager<ExtendedIdentityUser> _Manager;
		private ExtendedIdentityUser _User;

		public class ExtendedIdentityUser : IdentityUser
		{
			public string ExtendedField { get; set; }
		}

		[SetUp]
		public void BeforeEachTestAfterBase()
		{
			_Manager = CreateServiceProvider<ExtendedIdentityUser, IdentityRole>()
				.GetService<UserManager<ExtendedIdentityUser>>();
			_User = new ExtendedIdentityUser
			{
				UserName = "bob"
			};
		}

		[Test]
		public async Task Create_ExtendedUserType_SavesExtraFields()
		{
			_User.ExtendedField = "extendedField";

			await _Manager.CreateAsync(_User);

			var savedUser = Users.FindSync<ExtendedIdentityUser>(FilterDefinition<IdentityUser>.Empty).Single();
			Assert.AreEqual(savedUser.ExtendedField, "extendedField");
		}

		[Test]
		public async Task Create_ExtendedUserType_ReadsExtraFields()
		{
			_User.ExtendedField = "extendedField";

			await _Manager.CreateAsync(_User);

			var savedUser = await _Manager.FindByIdAsync(_User.Id);
			Assert.AreEqual(savedUser.ExtendedField, "extendedField");
		}
	}
}