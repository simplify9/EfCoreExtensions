using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SW.EfCoreExtensions
{
    public class CommonPropertiesBuilder
    {
        private readonly ModelBuilder modelBuilder;
        private readonly IEnumerable<IMutableEntityType> entityTypes;

        public CommonPropertiesBuilder(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
            entityTypes = modelBuilder.Model.GetEntityTypes().Where(e => !e.IsOwned()).ToList();
        }

        public CommonPropertiesBuilder HasAudit(byte userIdLength = 100)
        {
            foreach (var mutableEntityType in entityTypes)
                modelBuilder.Entity(mutableEntityType.ClrType, b => b.HasAudit(userIdLength));

            return this;
        }

        public CommonPropertiesBuilder HasSoftDeletionQueryFilter()
        {
            foreach (var mutableEntityType in entityTypes)
                modelBuilder.Entity(mutableEntityType.ClrType, b => b.HasSoftDeletionQueryFilter());
            
            return this;
        }

        public CommonPropertiesBuilder HasTenantForeignKey<TTenant>() where TTenant : class
        {
            foreach (var mutableEntityType in entityTypes)
                modelBuilder.Entity(mutableEntityType.ClrType, b => b.HasTenantForeignKey<TTenant>());
            
            return this;
        }

        public CommonPropertiesBuilder HasTenantQueryFilter(Expression<Func<int?>> tenantIdExpression)
        {
            foreach (var mutableEntityType in entityTypes)
                modelBuilder.Entity(mutableEntityType.ClrType, b => b.HasTenantQueryFilter(tenantIdExpression));
            
            return this;
        }

        //public static ModelBuilder CommonFeatures(this ModelBuilder modelBuilder, Type tenantEntity)
        //{
        //    foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
        //    {
        //        var type = mutableEntityType.ClrType;

        //        if (typeof(ISoftDelete).IsAssignableFrom(type))

        //            modelBuilder.Entity(type, b =>
        //            {
        //                //b.Property<bool>(nameof(ISoftDelete.Deleted));
        //                //b.HasQueryFilter((exp, item) => (EF.Property<bool>(item, "IsDeleted"), false););
        //                //b.HasQueryFilter(ConvertFilterExpression<ISoftDelete>(e => !e.Deleted, type));
        //            });

        //        if (typeof(IHasEntity).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {
        //                b.Property<string>(nameof(IHasEntity.Entity)).IsCode(10, true, false);
        //            });

        //        if (typeof(IHasOptionalEntity).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {
        //                b.Property<string>(nameof(IHasOptionalEntity.Entity)).IsCode(10, false, false);
        //            });

        //        if (typeof(IHasCreationTime).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {

        //            });

        //        if (typeof(ICreationAudited).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {
        //                b.Property<string>(nameof(ICreationAudited.CreatedBy)).IsCode(100, false, false);
        //            });

        //        if (typeof(IHasModificationTime).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {

        //            });

        //        if (typeof(IModificationAudited).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {
        //                b.Property<string>(nameof(IModificationAudited.ModifiedBy)).IsCode(100, false, false);
        //            });

        //        if (typeof(IHasDeletionTime).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {

        //            });

        //        if (typeof(IDeletionAudited).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {
        //                b.Property<string>(nameof(IDeletionAudited.DeletedBy)).IsCode(100, false, false);
        //            });

        //        if (tenantEntity != null && typeof(IHasTenant).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {
        //                b.HasOne(tenantEntity).WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Restrict);
        //            });

        //        if (tenantEntity != null && typeof(IHasOptionalTenant).IsAssignableFrom(type))
        //            modelBuilder.Entity(type, b =>
        //            {
        //                b.HasOne(tenantEntity).WithMany().HasForeignKey("TenantId").IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        //            });

        //    }

        //    return modelBuilder;
        //}
    }
}
