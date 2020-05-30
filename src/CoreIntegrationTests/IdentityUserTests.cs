namespace IntegrationTests	
{
    using Microsoft.AspNetCore.Identity.MongoDB;
    using MongoDB.Driver;
    using NUnit.Framework;

    [TestFixture]	
    public class IdentityUserTests : UserIntegrationTestsBase	
    {	
        [Test]	
        public void Insert_NoId_SetsId()	
        {	
            var user = new IdentityUser();	
            user.Id = null;	

            Users.InsertOne(user);
            var inserted = Users.Find(FilterDefinition<IdentityUser>.Empty).FirstOrDefault();
            Assert.NotNull(inserted);	
            Assert.Null(user.Id);	
        }	
    }	
}