using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.EfCoreExtensions.UnitTests.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.EfCoreExtensions.UnitTests
{
    [TestClass]
    public class EfCoreTests
    {
        static TestServer server;

        [ClassInitialize]
        public static void ClassInitialize(TestContext tcontext)
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
        async public Task TestSoftDeletionFilter()
        {
            using var scope = server.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var countTenants = await dbContext.Set<Tenant>().CountAsync();

            Assert.AreEqual(2, countTenants);
        }

        [TestMethod]
        async public Task TestSoftDeletionAndTenantFilters()
        {
            using var scope = server.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var countTickets = await dbContext.Set<Ticket>().CountAsync();

            Assert.AreEqual(1, countTickets);
        }

        [TestMethod]
        async public Task TestSoftDeletionAndOptionalTenantFilters()
        {
            using var scope = server.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var countTickets = await dbContext.Set<MetaData>().CountAsync();

            Assert.AreEqual(2, countTickets);
        }


    }
}
