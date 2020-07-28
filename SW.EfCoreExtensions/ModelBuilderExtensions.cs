using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SW.EfCoreExtensions
{
    public static class ModelBuilderExtensions
    {

        public static ModelBuilder BuildCommonProperties(this ModelBuilder modelBuilder) 
        {
            return BuildCommonProperties(modelBuilder, null);
        }
        public static ModelBuilder BuildCommonProperties(this ModelBuilder modelBuilder, Type tenantEntity) 
        {
            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                var type = mutableEntityType.ClrType;

                if (typeof(ISoftDelete).IsAssignableFrom(type))

                    modelBuilder.Entity(type, b =>
                    {
                        //b.Property<bool>(nameof(ISoftDelete.Deleted));
                        //b.HasQueryFilter((exp, item) => (EF.Property<bool>(item, "IsDeleted"), false););
                        //b.HasQueryFilter(ConvertFilterExpression<ISoftDelete>(e => !e.Deleted, type));
                    });

                if (typeof(IHasEntity).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(IHasEntity.Entity)).IsCode(10, true, false);
                    });

                if (typeof(IHasOptionalEntity).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(IHasOptionalEntity.Entity)).IsCode(10, false, false);
                    });

                if (typeof(IHasCreationTime).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {

                    });

                if (typeof(ICreationAudited).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(ICreationAudited.CreatedBy)).IsCode(100, false, false);
                    });

                if (typeof(IHasModificationTime).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {

                    });

                if (typeof(IModificationAudited).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(IModificationAudited.ModifiedBy)).IsCode(100, false, false);
                    });

                if (typeof(IHasDeletionTime).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {

                    });

                if (typeof(IDeletionAudited).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.Property<string>(nameof(IDeletionAudited.DeletedBy)).IsCode(100, false, false);
                    });

                if (tenantEntity != null && typeof(IHasTenant).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.HasOne(tenantEntity).WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Restrict);
                    });

                if (tenantEntity != null && typeof(IHasOptionalTenant).IsAssignableFrom(type))
                    modelBuilder.Entity(type, b =>
                    {
                        b.HasOne(tenantEntity).WithMany().HasForeignKey("TenantId").IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                    });

            }

            return modelBuilder;
        }

        public static void AddSequence<T>(this ModelBuilder modelBuilder)
        {
            if (modelBuilder.Model.FindEntityType(typeof(Sequence)) == null)
                modelBuilder.BuildSequenceTable();

            modelBuilder.Entity<Sequence>(b =>
            {
                b.HasData(new Sequence
                {
                    Entity = typeof(T).Name,
                    Value = 1,
                });
            });

        }
        private static void BuildSequenceTable(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sequence>(b =>
            {
                b.ToTable("Sequences");
                b.HasKey(p => p.Entity);
                b.Property(p => p.Entity).HasMaxLength(50);
            });
        }

        private static LambdaExpression ConvertFilterExpression<TInterface>(
                Expression<Func<TInterface, bool>> filterExpression,
                Type entityType)
        {
            var newParam = Expression.Parameter(entityType);
            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);

            return Expression.Lambda(newBody, newParam);
        }
    }


}
