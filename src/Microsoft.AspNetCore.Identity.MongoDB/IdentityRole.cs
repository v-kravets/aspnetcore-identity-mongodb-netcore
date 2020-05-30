namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using global::MongoDB.Bson;
    using System;

    public class IdentityRole
	{
        public IdentityRole()
            : this(() => ObjectId.GenerateNewId().ToString())
        {
        }
        public IdentityRole(Func<string> idGenerator)
        {
            Id = idGenerator();
        }

        public IdentityRole(string roleName) : this()
        {
            Name = roleName;
        }
        public IdentityRole(string roleName, Func<string> idGenerator) : this(idGenerator)
        {
            Name = roleName;
        }

        public string Id { get; set; }

		public string Name { get; set; }

		public string NormalizedName { get; set; }

		public override string ToString() => Name;
	}
}