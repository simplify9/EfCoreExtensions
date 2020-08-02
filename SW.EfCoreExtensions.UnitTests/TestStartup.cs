using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using SW.PrimitiveTypes;
using System.Security.Claims;

namespace SW.EfCoreExtensions.UnitTests
{
    public class TestStartup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<TestDbContext>(builder =>
            {
                var _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                builder.EnableSensitiveDataLogging();  
                builder.UseSqlite(_connection);
            },
            ServiceLifetime.Scoped,
            ServiceLifetime.Singleton);

            services.AddScoped(serviceProvider =>
            {
                ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "Adam"),
                        new Claim("TenantId", "1"),
                        new Claim("UserId", "1")
                    }));
                var requestContext = new RequestContext();
                requestContext.Set(user);
                return requestContext; 
            }); 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            dbContext.Database.EnsureCreated();
        }

        //private async Task PrepareDummyData(DbCtxt dbContext, IServiceScope serviceScope)
        //{

        //    var _form = new Form { Name = "Account Creation", Properties = new string[] { "Prop1" }, Url = "https://localhost:51512/app/CreateAccount" };
        //    dbContext.Set<Form>().Add(_form);
        //    await dbContext.SaveChangesAsync();

        //    var _Tenant = new Tenant
        //    {
        //        Code = "CAA",
        //        LegalName = "TestCAA",
        //        Name = "TestCAA",
        //        Address = new PrimitiveTypes.StreetAddress { City = "N/A", Country = "JO" }
        //    };
        //    dbContext.Set<Tenant>().Add(_Tenant);
        //    await dbContext.SaveChangesAsync();

        //   dbContext.Set<FormConfig>().Add(new FormConfig
        //    {
        //        Name = _form.Name,
        //        FormId = _form.Id,
        //        Approvers = new List<FormApprover>
        //       {
        //        new FormApprover{ Order=1, Type= FormApproverType.PersonId, Value="1"},
        //        new FormApprover{ Order=2, Type= FormApproverType.PersonId, Value="2"},
        //        new FormApprover{ Order=3, Type= FormApproverType.PersonId, Value="3"}
        //       }
        //    });
        //    await dbContext.SaveChangesAsync();

        //    var data = dbContext.Set<FormConfig>();

        //    var _person = new Person
        //    {
        //        //CompanyId = companyid,

        //        FirstName = "shady",
        //        LastName = "arafeh",
        //        Name = "shady arafeh",
        //        Address = new PrimitiveTypes.StreetAddress()
        //        {
        //            City = "N/A",
        //            Country = _Tenant.Address.Country
        //        },
        //        AccountId = "1234",
        //        Email = "shady@aramex.com",
        //        Phone = "1234568",
        //        Roles = new int[] { Role.BasicUser }
        //    };

        //    var _person1 = new Person
        //    {
        //        //CompanyId = companyid,
        //        FirstName = "shady1",
        //        LastName = "arafeh1",
        //        Name = "shady arafeh1",
        //        Address = new PrimitiveTypes.StreetAddress()
        //        {
        //            City = "N/A",
        //            Country = _Tenant.Address.Country
        //        },
        //        AccountId = "1234",
        //        Email = "sarafeh@hotmail.com",
        //        Phone = "1234568",
        //        Roles = new int[] { Role.BasicUser }
        //    };

        //    var _person2 = new Person
        //    {

        //        //CompanyId = companyid,
        //        FirstName = "shady2",
        //        LastName = "arafeh2",
        //        Name = "shady arafeh2",
        //        Address = new PrimitiveTypes.StreetAddress()
        //        {
        //            City = "N/A",
        //            Country = _Tenant.Address.Country
        //        },
        //        AccountId = "1234",
        //        Email = "shadyarafeh99@gmail.com",
        //        Phone = "1234568",
        //        Roles = new int[] { Role.BasicUser }
        //    };
        //    Person[] persons = { _person, _person1, _person2 };
        //    dbContext.Set<Person>().AddRange(persons);
        //    await dbContext.SaveChangesAsync();
        //}

    }
}
