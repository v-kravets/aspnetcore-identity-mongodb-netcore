namespace IntegrationTests
{
	using System;
	using System.Diagnostics;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class IndexChecksTests : UserIntegrationTestsBase
	{
		[Test]
		public void EnsureUniqueIndexes()
		{
			EnsureUniqueIndex<IdentityUser>(IndexChecks.OptionalIndexChecks.EnsureUniqueIndexOnUserName, "UserName");
			EnsureUniqueIndex<IdentityUser>(IndexChecks.OptionalIndexChecks.EnsureUniqueIndexOnEmail, "Email");
			EnsureUniqueIndex<IdentityRole>(IndexChecks.OptionalIndexChecks.EnsureUniqueIndexOnRoleName, "Name");

			EnsureUniqueIndex<IdentityUser>(IndexChecks.EnsureUniqueIndexOnNormalizedUserName, "NormalizedUserName");
			EnsureUniqueIndex<IdentityUser>(IndexChecks.EnsureUniqueIndexOnNormalizedEmail, "NormalizedEmail");
			EnsureUniqueIndex<IdentityRole>(IndexChecks.EnsureUniqueIndexOnNormalizedRoleName, "NormalizedName");
		}

		private void EnsureUniqueIndex<TCollection>(Action<IMongoCollection<TCollection>> addIndex, string indexedField)
		{
			var testCollectionName = "indextest";
			Database.DropCollection(testCollectionName);
			var testCollection = DatabaseNewApi.GetCollection<TCollection>(testCollectionName);

			addIndex(testCollection);

            System.Threading.Thread.Sleep(500);

			var legacyCollectionInterface = Database.GetCollection<TCollection>(testCollectionName);
			var indexes = legacyCollectionInterface.Indexes.List().ToList();
			foreach (var index in indexes)
			{
				Trace.WriteLine(index.ToJson());
			}

			// todo fix index verification code
			// indexes.Where(i => i.IsUnique)
			//        .Where(i => i.Key.Count() == 1)
			//        .FirstOrDefault(i => i.Key.Contains(indexedField));
			// var failureMessage = $"No unique index found on {indexedField}";
			// Assert.AreEqual(index, Is.Not.Null, failureMessage);
			// Assert.AreEqual(index.Key.Count(), Is.EqualTo(1), failureMessage);
		}
	}
}