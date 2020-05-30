namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using global::MongoDB.Driver;

	public static class IndexChecks
	{
		public static void EnsureUniqueIndexOnNormalizedUserName<TUser>(IMongoCollection<TUser> users)
			where TUser : IdentityUser
		{
			var userName = Builders<TUser>.IndexKeys.Ascending(t => t.NormalizedUserName);
			var options = new CreateIndexOptions {Unique = true};
			var indexModel = new CreateIndexModel<TUser>(userName.Ascending(x => x.NormalizedUserName), options);
			users.Indexes.CreateOneAsync(indexModel);
		}

		public static void EnsureUniqueIndexOnNormalizedRoleName<TRole>(IMongoCollection<TRole> roles)
			where TRole : IdentityRole
		{
			var roleName = Builders<TRole>.IndexKeys.Ascending(t => t.NormalizedName);
			var options = new CreateIndexOptions {Unique = true};
			var indexModel = new CreateIndexModel<TRole>(roleName.Ascending(x => x.NormalizedName), options);
			roles.Indexes.CreateOneAsync(indexModel);
		}

		public static void EnsureUniqueIndexOnNormalizedEmail<TUser>(IMongoCollection<TUser> users)
			where TUser : IdentityUser
		{
			var email = Builders<TUser>.IndexKeys.Ascending(t => t.NormalizedEmail);
			var options = new CreateIndexOptions {Unique = true};
			var indexModel = new CreateIndexModel<TUser>(email.Ascending(x => x.NormalizedEmail), options);
			users.Indexes.CreateOneAsync(indexModel);
		}

		/// <summary>
		///     ASP.NET Core Identity now searches on normalized fields so these indexes are no longer required, replace with
		///     normalized checks.
		/// </summary>
		public static class OptionalIndexChecks
		{
			public static void EnsureUniqueIndexOnUserName<TUser>(IMongoCollection<TUser> users)
				where TUser : IdentityUser
			{
				var userName = Builders<TUser>.IndexKeys.Ascending(t => t.UserName);
				var options = new CreateIndexOptions {Unique = true};
				var indexModel = new CreateIndexModel<TUser>(userName.Ascending(x => x.UserName), options);
				users.Indexes.CreateOneAsync(indexModel);
			}

			public static void EnsureUniqueIndexOnRoleName<TRole>(IMongoCollection<TRole> roles)
				where TRole : IdentityRole
			{
				var roleName = Builders<TRole>.IndexKeys.Ascending(t => t.Name);
				var options = new CreateIndexOptions {Unique = true};
				var indexModel = new CreateIndexModel<TRole>(roleName.Ascending(x => x.Name), options);
				roles.Indexes.CreateOneAsync(indexModel);
			}

			public static void EnsureUniqueIndexOnEmail<TUser>(IMongoCollection<TUser> users)
				where TUser : IdentityUser
			{
				var email = Builders<TUser>.IndexKeys.Ascending(t => t.Email);
				var options = new CreateIndexOptions {Unique = true};
				var indexModel = new CreateIndexModel<TUser>(email.Ascending(x => x.Email), options);
				users.Indexes.CreateOneAsync(indexModel);
			}
		}
	}
}