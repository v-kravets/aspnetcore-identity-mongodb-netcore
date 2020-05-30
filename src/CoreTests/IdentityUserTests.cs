namespace Tests
{
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Bson;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class IdentityUserTests
	{
		[Test]
		public void Create_NewIdentityUser_HasIdAssigned()
		{
			var user = new IdentityUser();

			var parsed = user.Id.SafeParseObjectId();
			Assert.NotNull(parsed);
			Assert.AreNotEqual(parsed, ObjectId.Empty);
		}

		[Test]
		public void Create_NoPassword_DoesNotSerializePasswordField()
		{
			// if a particular consuming application doesn't intend to use passwords, there's no reason to store a null entry except for padding concerns, if that is the case then the consumer may want to create a custom class map to serialize as desired.

			var user = new IdentityUser();

			var document = user.ToBsonDocument();

			Assert.False(document.Contains("PasswordHash"));
		}

		[Test]
		public void Create_NullLists_DoesNotSerializeNullLists()
		{
			// serialized nulls can cause havoc in deserialization, overwriting the constructor's initial empty list 
			var user = new IdentityUser();
			user.Roles = null;
			user.Tokens = null;
			user.Logins = null;
			user.Claims = null;

			var document = user.ToBsonDocument();

			Assert.False(document.Contains("Roles"));
			Assert.False(document.Contains("Tokens"));
			Assert.False(document.Contains("Logins"));
			Assert.False(document.Contains("Claims"));
		}

		[Test]
		public void Create_NewIdentityUser_ListsNotNull()
		{
			var user = new IdentityUser();

			Assert.AreEqual(user.Logins, string.Empty);
			Assert.AreEqual(user.Tokens, string.Empty);
			Assert.AreEqual(user.Roles, string.Empty);
			Assert.AreEqual(user.Claims, string.Empty);
		}
	}
}