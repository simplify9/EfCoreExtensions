using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SW.EfCoreExtensions
{
    public static class EntityTypeBuilderExtensions
    {

        public static EntityTypeBuilder AddQueryFilter(this EntityTypeBuilder entityTypeBuilder, Expression<Func<object, bool>> expression)
        {
            return AddQueryFilter<object>(entityTypeBuilder, expression);
        }

        public static EntityTypeBuilder AddQueryFilter<TFilter>(this EntityTypeBuilder entityTypeBuilder, Expression<Func<TFilter, bool>> expression)
        {
            var parameterType = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);
            var expressionFilter = ReplacingExpressionVisitor.Replace(
                expression.Parameters.Single(), parameterType, expression.Body);
            
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

        //public static EntityTypeBuilder AddQueryFilter<TEntity>(this EntityTypeBuilder entityTypeBuilder, Expression<Func<TEntity, bool>> expression) where TEntity : class
        //{
        //    var parameterType = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);
        //    var expressionFilter = ReplacingExpressionVisitor.Replace(
        //        expression.Parameters.Single(), parameterType, expression.Body);

        //    var currentQueryFilter = entityTypeBuilder.Metadata.GetQueryFilter();
        //    if (currentQueryFilter != null)
        //    {
        //        var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
        //            currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
        //        expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
        //    }

        //    var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
        //    entityTypeBuilder.HasQueryFilter(lambdaExpression);

        //    return entityTypeBuilder;
        //}

    }
}
