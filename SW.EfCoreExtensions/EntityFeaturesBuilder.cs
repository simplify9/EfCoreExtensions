using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SW.PrimitiveTypes;
using System;
using System.Linq.Expressions;

namespace SW.EfCoreExtensions
{
    public class EntityFeaturesBuilder<TEntity> : EntityFeaturesBuilder where TEntity : class
    {

        public EntityFeaturesBuilder(ModelBuilder modelBuilder) : base(modelBuilder, typeof(TEntity))
        {
        }

        public EntityFeaturesBuilder<TEntity> HasSequenceGenerator<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            //base.HasSequenceGenerator<TProperty>(propertyExpression.Compile().Invoke(TEn))
            //entityBuilder.Property(propertyExpression).ValueGeneratedOnAdd().HasValueGenerator<SequenceValueGenerator>();

            //if (modelBuilder.Model.FindEntityType(typeof(Sequence)) == null)
            //    BuildSequenceTable();

            //modelBuilder.Entity<Sequence>(b =>
            //{
            //    b.HasData(new Sequence
            //    {
            //        Entity = typeof(TEntity).Name,
            //        Value = 1,
            //    });
            //});

            return this;
        }

        //public new EntityFeaturesBuilder<TEntity> HasTenantForeignKey<TTenant>() where TTenant : class
        //{
        //    base.HasTenantForeignKey<TTenant>();//.HasOne<TTenant>().WithMany().HasForeignKey(nameof(IHasTenant.TenantId)).IsRequired(!optional).OnDelete(DeleteBehavior.Restrict);
        //    return this;
        //}

        //public new EntityFeaturesBuilder<TEntity> HasSoftDeletionQueryFilter()
        //{
        //    base.HasSoftDeletionQueryFilter();
        //    return this;
        //}

        //public new EntityFeaturesBuilder<TEntity> HasTenantQueryFilter(RequestContext requestContext)
        //{
        //    base.HasTenantQueryFilter(requestContext);

        //    return this;
        //}

    }

    public class EntityFeaturesBuilder
    {
        private readonly ModelBuilder modelBuilder;
        private readonly Type entityType;
        private readonly EntityTypeBuilder entityBuilder;

        public EntityFeaturesBuilder(ModelBuilder modelBuilder, Type entityType)
        {
            this.modelBuilder = modelBuilder;
            this.entityType = entityType;
            entityBuilder = modelBuilder.Entity(entityType);
        }

        public EntityFeaturesBuilder HasSequenceGenerator<TProperty>(string propertyName)
        {
            entityBuilder.Property<TProperty>(propertyName).ValueGeneratedOnAdd().HasValueGenerator<SequenceValueGenerator>();

            if (modelBuilder.Model.FindEntityType(typeof(Sequence)) == null)
                BuildSequenceTable();

            modelBuilder.Entity<Sequence>(b =>
            {
                b.HasData(new Sequence
                {
                    Entity = entityType.Name,
                    Value = 1,
                });
            });

            return this;
        }

        public EntityFeaturesBuilder HasTenantForeignKey<TTenant>() where TTenant : class
        {
            if (typeof(IHasOptionalTenant).IsAssignableFrom(entityType) || typeof(IHasTenant).IsAssignableFrom(entityType))
                entityBuilder.HasOne(typeof(TTenant)).WithMany().HasForeignKey(nameof(IHasTenant.TenantId)).OnDelete(DeleteBehavior.Restrict);
            return this;
        }

        public EntityFeaturesBuilder HasSoftDeletionQueryFilter()
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType))
                entityBuilder.AddQueryFilter<ISoftDelete>(item => !item.Deleted);

            return this;
        }

        public EntityFeaturesBuilder HasTenantQueryFilter(RequestContext requestContext)
        {
            //bool tenantIdNullable = Nullable.GetUnderlyingType(entityType.GetProperty("TenantId").PropertyType) != null;
            if (typeof(IHasOptionalTenant).IsAssignableFrom(entityType))
                entityBuilder.AddQueryFilter<IHasOptionalTenant>(item => item.TenantId == requestContext.GetTenantId() || item.TenantId == null);
            else if (typeof(IHasTenant).IsAssignableFrom(entityType))
                entityBuilder.AddQueryFilter<IHasTenant>(item => item.TenantId == requestContext.GetTenantId().Value);

            return this;
        }

        public EntityFeaturesBuilder HasAudit(byte userIdLength = 100)
        {
            if (typeof(ICreationAudited).IsAssignableFrom(entityType))
                modelBuilder.Entity(entityType, b =>
                {
                    b.Property<string>(nameof(ICreationAudited.CreatedBy)).IsCode(userIdLength, false, false);
                });

            if (typeof(IModificationAudited).IsAssignableFrom(entityType))
                modelBuilder.Entity(entityType, b =>
                {
                    b.Property<string>(nameof(IModificationAudited.ModifiedBy)).IsCode(userIdLength, false, false);
                });

            if (typeof(IDeletionAudited).IsAssignableFrom(entityType))
                modelBuilder.Entity(entityType, b =>
                {
                    b.Property<string>(nameof(IDeletionAudited.DeletedBy)).IsCode(userIdLength, false, false);
                });

            return this;
        }

        private void BuildSequenceTable()
        {
            modelBuilder.Entity<Sequence>(b =>
            {
                b.ToTable("Sequences");
                b.HasKey(p => p.Entity);
                b.Property(p => p.Entity).HasMaxLength(50);
            });
        }
    }
}
