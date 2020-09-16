using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus.DataSets;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;

namespace SW.EfCoreExtensions.UnitTests
{
    [TestClass]
    public class DataInsertionTests
    {

        //private DatabaseFacade facade;
        static TestServer server;

        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx)
        {
            server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseDefaultServiceProvider((context, options) => { options.ValidateScopes = true; })
                .UseEnvironment("Development")
                .UseStartup<TestStartup>());

        }
        
        [ClassCleanup]
        public static void ClassCleanup()
        {
            server.Dispose();
        }
                
        
        [TestMethod]
        public async Task TestAddAndDeleteOne()
        {
            Bag bag = new Bag
            {
                Description = "Test desc",
                Closed = true
            };

            var scope = server.Services.CreateScope();
            var facade = scope.ServiceProvider.GetService<TestDbContext>().Database;

            await facade.Add("Bags", bag);

            var all = await facade.All<Bag>("Bags", new List<SearchyCondition>());
            Assert.AreEqual(all.Count(), 1);

            await facade.Delete<Bag>("Bags", new SearchyCondition[]
                {new SearchyCondition(nameof(Bag.Description), SearchyRule.EqualsTo, "Test desc")});
            all = await facade.All<Bag>("Bags", new List<SearchyCondition>());
            Assert.
                AreEqual(all.Count(), 0);
        }

        [TestMethod]
        public async Task TestAddMultiple()
        {
            var scope = server.Services.CreateScope();
            var facade = scope.ServiceProvider.GetService<TestDbContext>().Database;
            
            
            Bag bag = new Bag
            {
                Description = "Test desc",
                Number = "121",
                Closed = true,
                Entity = "1"
            };
            
            Bag bag2 = new Bag
            {
                Description = "Test desc2",
                Number = "1212",
                Closed = true,
                Entity = "2"
            };
            
            await facade.Add("Bags", bag);
            await facade.Add("Bags", bag2);
            var all = await facade.All<Bag>("Bags", new List<SearchyCondition>());
            
            Assert.AreEqual(all.Count(), 2);
            await facade.Delete<Bag>("Bags", new SearchyCondition[]
                {new SearchyCondition("Entity", SearchyRule.EqualsTo, "2")});
            await facade.Delete<Bag>("Bags", new SearchyCondition[]
                {new SearchyCondition("Entity", SearchyRule.EqualsTo, "1")});
            all = await facade.All<Bag>("Bags", new List<SearchyCondition>());
            Assert.AreEqual(all.Count(), 0);
        }
        
        [TestMethod]
        public async Task TestUpdateOne()
        {
            var scope = server.Services.CreateScope();
            var facade = scope.ServiceProvider.GetService<TestDbContext>().Database;
            
            Bag bag = new Bag
            {
                Id = 10,
                Description = "500",
                Closed = true
            };
            await facade.Add("Bags", bag);
            await facade.Update("Bags", new Bag
            {
                Id = 10,
                Closed = false
            });

            var updatedRecord = await facade.One<Bag>("Bags", 10);
            
            if(!facade.IsSqlite())
                Assert.AreEqual(updatedRecord.Closed,false);
        }
    }
}