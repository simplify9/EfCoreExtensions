using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.EfCoreExtensions
{
    public class CommonFeaturesBuilder
    {
        private readonly ModelBuilder modelBuilder;
        private readonly IEnumerable<IMutableEntityType> entityTypes;

        public CommonFeaturesBuilder(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
            entityTypes = modelBuilder.Model.GetEntityTypes();
        }

        public CommonFeaturesBuilder HasAuditFeatures(byte userIdLength = 100)
        {
            foreach (var mutableEntityType in entityTypes)
            {
                var type = mutableEntityType.ClrType;

                if (typeof(ICreationAudited).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(ICreationAudited.CreatedBy)).IsCode(userIdLength, false, false);
                    });

                if (typeof(IModificationAudited).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(IModificationAudited.ModifiedBy)).IsCode(userIdLength, false, false);
                    });

                if (typeof(IDeletionAudited).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(IDeletionAudited.DeletedBy)).IsCode(userIdLength, false, false);
                    });
            }

            return this;

        }

        public CommonFeaturesBuilder HasSoftDeletionQueryFilter()
        {
            return this;
        }

        public CommonFeaturesBuilder HasTenantForeignKey<TTenant>() where TTenant : class
        {
            foreach (var mutableEntityType in entityTypes)
            {
                var type = mutableEntityType.ClrType;
            }
            return this;
        }

        public CommonFeaturesBuilder HasTenantQueryFilter(RequestContext requestContext)
        {
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
