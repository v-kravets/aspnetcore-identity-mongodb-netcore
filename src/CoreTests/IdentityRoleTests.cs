﻿namespace Tests
{
	using System.Reflection;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using MongoDB.Bson;
	using NUnit.Framework;

	// todo low - validate all tests work
	[TestFixture]
	public class IdentityRoleTests
	{
		[Test]	
		public void ToBsonDocument_IdAssigned_MapsToBsonString()	
		{	
			var role = new IdentityRole();	

			var document = role.ToBsonDocument();	

			Assert.AreEqual(document["_id"].GetType(), typeof(MongoDB.Bson.BsonString));	
		}
		
		[Test]
		public void Create_WithoutRoleName_HasIdAssigned()
		{
			var role = new IdentityRole();

			var parsed = role.Id.SafeParseObjectId();
			Assert.NotNull(parsed);
			Assert.AreNotEqual(parsed, ObjectId.Empty);
		}

		[Test]
		public void Create_WithRoleName_SetsName()
		{
			var name = "admin";

			var role = new IdentityRole(name);

			Assert.AreEqual(role.Name, name);
		}

		[Test]
		public void Create_WithRoleName_SetsId()
		{
			var role = new IdentityRole("admin");

			var parsed = role.Id.SafeParseObjectId();
			Assert.NotNull(parsed);
			Assert.AreNotEqual(parsed, ObjectId.Empty);
		}
	}
}