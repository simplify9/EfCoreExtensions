using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using SW.PrimitiveTypes;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SW.EfCoreExtensions
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder HasTenantForeignKey<TTenant>(this EntityTypeBuilder entityTypeBuilder) where TTenant : class
        {
            if (typeof(IHasOptionalTenant).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType) || typeof(IHasTenant).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
                entityTypeBuilder.HasOne(typeof(TTenant)).WithMany().HasForeignKey(nameof(IHasTenant.TenantId)).OnDelete(DeleteBehavior.Restrict);
            return entityTypeBuilder;
        }

        public static EntityTypeBuilder HasSoftDeletionQueryFilter(this EntityTypeBuilder entityTypeBuilder)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
                entityTypeBuilder.AddQueryFilter<ISoftDelete>(item => !item.Deleted);

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder HasTenantQueryFilter(this EntityTypeBuilder entityTypeBuilder, Expression<Func<int?>> tenantIdExpression)
        {
            if (typeof(IHasTenant).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
            {
                var param = Expression.Parameter(typeof(IHasTenant), "TFilter");
                var fieldNameExpression = Expression.Property(param, nameof(IHasTenant.TenantId));
                var equalExpr = Expression.Equal(fieldNameExpression, Expression.Convert(tenantIdExpression.Body, typeof(int)));
                var funcExp = Expression.Lambda<Func<IHasTenant, bool>>(equalExpr, param);
                entityTypeBuilder.AddQueryFilter(funcExp);

            }
            else if (typeof(IHasOptionalTenant).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
            {
                var param = Expression.Parameter(typeof(IHasOptionalTenant), "TFilter");
                var fieldNameExpression = Expression.Property(param, nameof(IHasOptionalTenant.TenantId));
                var equalExpr = Expression.Equal(fieldNameExpression, tenantIdExpression.Body);
                var orExp = Expression.OrElse(equalExpr, Expression.Equal(fieldNameExpression, Expression.Constant(null)));
                var funcExp = Expression.Lambda<Func<IHasOptionalTenant, bool>>(orExp, param);
                entityTypeBuilder.AddQueryFilter(funcExp);
            }

            return entityTypeBuilder;
        }

        //public static EntityTypeBuilder HasTenantQueryFilter(this EntityTypeBuilder entityTypeBuilder, RequestContext requestContext)
        //{
        //    //bool tenantIdNullable = Nullable.GetUnderlyingType(entityType.GetProperty("TenantId").PropertyType) != null;
        //    if (typeof(IHasOptionalTenant).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
        //        entityTypeBuilder.AddQueryFilter<IHasOptionalTenant>(item => item.TenantId == requestContext.GetTenantId() || item.TenantId == null);
        //    else if (typeof(IHasTenant).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
        //        entityTypeBuilder.AddQueryFilter<IHasTenant>(item => item.TenantId == requestContext.GetTenantId().Value);

        //    return entityTypeBuilder;
        //}

        public static EntityTypeBuilder HasAudit(this EntityTypeBuilder entityTypeBuilder, byte userIdLength = 100)
        {
            if (typeof(ICreationAudited).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
                entityTypeBuilder.Property<string>(nameof(ICreationAudited.CreatedBy)).IsCode(userIdLength, false, false);

            if (typeof(IModificationAudited).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
                entityTypeBuilder.Property<string>(nameof(IModificationAudited.ModifiedBy)).IsCode(userIdLength, false, false);

            if (typeof(IDeletionAudited).IsAssignableFrom(entityTypeBuilder.Metadata.ClrType))
                entityTypeBuilder.Property<string>(nameof(IDeletionAudited.DeletedBy)).IsCode(userIdLength, false, false);

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder AddQueryFilter<TFilter>(this EntityTypeBuilder entityTypeBuilder, Expression<Func<TFilter, bool>> expression)
        {
            var parameterType = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);
            var expressionFilter = ReplacingExpressionVisitor.Replace(
                expression.Parameters.First(), parameterType, expression.Body);

            var currentQueryFilter = entityTypeBuilder.Metadata.GetQueryFilter();
            if (currentQueryFilter != null)
            {
                var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                    currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
                expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
            }

            var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
            entityTypeBuilder.HasQueryFilter(lambdaExpression);

            return entityTypeBuilder;
        }
    }
}
