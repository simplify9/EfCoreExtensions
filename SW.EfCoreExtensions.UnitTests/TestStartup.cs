using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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


    }
}
