using Microsoft.EntityFrameworkCore;
using SW.EfCoreExtensions.UnitTests.Domain;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.EfCoreExtensions.UnitTests
{
    class TestDbContext : DbContext
    {
        private readonly RequestContext requestContext;

        public TestDbContext(DbContextOptions options, RequestContext requestContext) : base(options)
        {
            this.requestContext = requestContext;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>(b =>
            {
                b.ToTable("Tenants");
                b.Property(p => p.Logo).StoreAsJson().IsUnicode(false).IsRequired(true).HasMaxLength(1000);
                b.HasData(new
                {
                    Id = 1,
                    Name = "Tenant 1",
                    Deleted = false,
                    Logo = new RemoteBlob()
                }, new
                {
                    Id = 2,
                    Name = "Tenant 2",
                    Deleted = false,
                    Logo = new RemoteBlob()
                }, new
                {
                    Id = 3,
                    Name = "Tenant 3",
                    Deleted = true,
                    Logo = new RemoteBlob()
                });
            });

            modelBuilder.Entity<Ticket>(b =>
            {
                b.ToTable("Tickets");
                b.Property(p => p.Number).HasSequenceGenerator();

                b.HasData(new
                {
                    Id = 1,
                    Number = 1,
                    TenantId = 1,
                    Deleted = false
                }, new
                {
                    Id = 2,
                    Number = 2,
                    TenantId = 1,
                    Deleted = true
                }, new
                {
                    Id = 3,
                    Number = 3,
                    TenantId = 2,
                    Deleted = false
                });
            });

            modelBuilder.Entity<MetaData>(b =>
            {
                b.ToTable("Metadata");
                b.HasData(new
                {
                    Id = 1,
                    Name = "meta1",
                    Value = "value1",
                    //TenantId = null,
                }, new
                {
                    Id = 2,
                    Name = "meta2",
                    Value = "value2",
                    TenantId = 1,
                }, new
                {
                    Id = 3,
                    Number = 3,
                    TenantId = 2,
                });
            });


            modelBuilder.Entity<SomeData>(b =>
            {
                b.ToTable("SomeData");
                b.HasKey(x => x.Id);
                b.Property(x => x.StringArray).IsSeparatorDelimited(',');
                b.Property(x => x.IntegerArray).IsSeparatorDelimited(',');

                b.HasData(new
                {
                    Id = 1,
                    StringArray = new string[] { "a", "b" , "c" },
                    IntegerArray = new int[] {1,2,3}
                },new
                {
                    Id = 2,
                    StringArray = new string[] { "d", "e" , "f" },
                    IntegerArray = new int[] {4,5,6}
                });
            });
            modelBuilder.CommonProperties(b =>
            {
                b.HasSoftDeletionQueryFilter();
                b.HasTenantQueryFilter(() => requestContext.GetTenantId());
                b.HasTenantForeignKey<Tenant>();
                b.HasAudit();
            });
        }
    }
}
