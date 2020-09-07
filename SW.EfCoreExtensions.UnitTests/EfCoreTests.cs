using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.EfCoreExtensions.UnitTests.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore.Internal;

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
        async public Task TestIsSeparatorDelimitedString()
        {
            using var scope = server.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var first = await dbContext.Set<SomeData>().FirstOrDefaultAsync();
            var expected = new[]{ "a","b","c"};
            Assert.IsTrue(expected.SequenceEqual(first.StringArray));
            await using var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = $"SELECT * From {nameof(SomeData)}";
            await dbContext.Database.OpenConnectionAsync();
            await using var result = await command.ExecuteReaderAsync();
            await result.ReadAsync();
            Assert.AreEqual("a,b,c", result.GetString(1));
            var readByWhereEquals = await dbContext.Set<SomeData>().FirstOrDefaultAsync(
                x => x.StringArray == new[] {"d", "e", "f"});
            Assert.IsNotNull(readByWhereEquals);
            Assert.IsTrue(readByWhereEquals.StringArray.Contains("e"));
            var toInsert = new SomeData {StringArray = new[] {"g", "h", "i"}};
            dbContext.Add(toInsert);
            await dbContext.SaveChangesAsync();
            var inserted = await dbContext.Set<SomeData>()
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            Assert.IsTrue(toInsert.StringArray.SequenceEqual(inserted.StringArray));
            
            var toInsertNull = new SomeData();
            dbContext.Add(toInsertNull);
            await dbContext.SaveChangesAsync();
            var insertedNull = await dbContext.Set<SomeData>()
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            Assert.IsNull(insertedNull.StringArray);
            
            var commandInsertManually = $"INSERT INTO SomeData ({nameof(inserted.StringArray)}) VALUES ('j,k,l')";
            await dbContext.Database.ExecuteSqlRawAsync(commandInsertManually);
            var insertedManually = await dbContext.Set<SomeData>()
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            Assert.IsTrue(insertedManually.StringArray.SequenceEqual(new []{"j","k","l"}));

        }
        
        [TestMethod]
        async public Task TestIsSeparatorDelimitedInteger()
        {
            using var scope = server.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var first = await dbContext.Set<SomeData>().FirstOrDefaultAsync();
            var expected = new[]{ 1,2,3 };
            Assert.IsTrue(expected.SequenceEqual(first.IntegerArray));
            await using var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = $"SELECT * From {nameof(SomeData)}";
            await dbContext.Database.OpenConnectionAsync();
            await using var result = await command.ExecuteReaderAsync();
            await result.ReadAsync();
            Assert.AreEqual("1,2,3", result.GetString(2));
            var readByWhereEquals = await dbContext.Set<SomeData>().FirstOrDefaultAsync(
                x => x.IntegerArray == new[] {4, 5, 6});
            Assert.IsNotNull(readByWhereEquals);
            Assert.IsTrue(readByWhereEquals.IntegerArray.Contains(4));
            
            var toInsert = new SomeData {IntegerArray = new[] {7,8,9}};
            dbContext.Add(toInsert);
            await dbContext.SaveChangesAsync();
            var inserted = await dbContext.Set<SomeData>()
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            Assert.IsTrue(toInsert.IntegerArray.SequenceEqual(inserted.IntegerArray));
            
            var toInsertNull = new SomeData();
            dbContext.Add(toInsertNull);
            await dbContext.SaveChangesAsync();
            var insertedNull = await dbContext.Set<SomeData>()
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            Assert.IsNull(insertedNull.IntegerArray);
            
            var commandInsertManually = $"INSERT INTO SomeData ({nameof(inserted.IntegerArray)}) VALUES ('10,11,12')";
            await dbContext.Database.ExecuteSqlRawAsync(commandInsertManually);
            var insertedManually = await dbContext.Set<SomeData>()
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            Assert.IsTrue(insertedManually.IntegerArray.SequenceEqual(new[]{ 10,11,12 }));

            
        }
        [TestMethod]
        async public Task TestSoftDeletionAndOptionalTenantFilters()
        {
            using var scope = server.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var countTickets = await dbContext.Set<MetaData>().CountAsync();

            Assert.AreEqual(2, countTickets);
        }

        [TestMethod]
        async public Task TestSequenceGenerator()
        {
            using var scope = server.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

            var ticket = new Ticket();
            await dbContext.AddAsync(ticket);

            Assert.AreEqual(1, ticket.Number);

            var ticket2 = new Ticket();
            await dbContext.AddAsync(ticket2);

            Assert.AreEqual(2, ticket2.Number);
        }


    }
}
