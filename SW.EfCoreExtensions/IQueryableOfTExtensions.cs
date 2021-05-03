using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SW.PrimitiveTypes;

namespace SW.EfCoreExtensions
{
    public static class IQueryableOfTExtensions
    {

        async public static Task<IDictionary<string, string>> ToDictionaryAsync<TEntity>(this IQueryable<TEntity> query, SearchyRequest searchyRequest, Func<TEntity, string> keySelector, Func<TEntity, string> valueSelector = null)
        {
            return await query.
                Search(searchyRequest.Conditions).
                ToDictionaryAsync(keySelector, valueSelector ?? keySelector);
        }

        async public static Task<SearchyResponse<TEntity>> ToSearchyResponseAsync<TEntity>(this IQueryable<TEntity> query, SearchyRequest searchyRequest)
        {
            return new SearchyResponse<TEntity>
            {
                Result = await query.Search(searchyRequest.Conditions, searchyRequest.Sorts, searchyRequest.PageSize, searchyRequest.PageIndex).ToListAsync(),
                TotalCount = searchyRequest.CountRows ? await query.Search(searchyRequest.Conditions).CountAsync() : 0
            };
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target, string field, object valueEquals)
        {
            return Search(target, new SearchyCondition[] { new SearchyCondition(field, SearchyRule.EqualsTo, valueEquals) });
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target, string field, SearchyRule rule, object value)
        {
            return Search(target, new SearchyCondition[] { new SearchyCondition(field, rule, value) });
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target, SearchyCondition condition)
        {
            return Search(target, new SearchyCondition[] { condition });
        }
        public static IQueryable<TEntity> If<TEntity, TProperty>(this IIncludableQueryable<TEntity, TProperty> source, bool condition, Func<IIncludableQueryable<TEntity, TProperty>, IQueryable<TEntity>> transform) where TEntity : class
        {
            return condition ? transform(source) : source;
        }
        public static IQueryable<TEntity> If<TEntity>(this IQueryable<TEntity> source, bool condition, Func<IQueryable<TEntity>, IQueryable<TEntity>> transform)
        {
            return condition ? transform(source) : source;
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target,
            IEnumerable<SearchyCondition> conditions,
            IEnumerable<SearchySort> orders = null,
            int pageSize = 0,
            int pageIndex = 0)
        {

            var param = Expression.Parameter(typeof(TEntity), "TEntity");
            var finalexp = param.BuildConditionsExpression<TEntity>(conditions);

            if (finalexp != null)
            {
                var finalwhereexp = Expression.Lambda<Func<TEntity, bool>>(finalexp, param);
                target = target.Where(finalwhereexp);
            }

            if (orders != null && orders.Count() > 0)
            {
                var mainOrderBy = orders.FirstOrDefault();
                var mainSortMemberType = typeof(TEntity).GetProperty(mainOrderBy.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;

                target = target.BuildOrderByThenBy(mainOrderBy, mainSortMemberType, true);

                var searchySorts = new List<SearchySort>
                {
                    mainOrderBy
                };

                foreach (var searchySort in orders.Except(searchySorts.AsEnumerable()))
                {
                    var sortMemberType = typeof(TEntity).GetProperty(searchySort.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    target = target.BuildOrderByThenBy(searchySort, sortMemberType, false);
                }
            }

            if (pageSize > 0 & pageIndex > 0)
                target = target.Skip(pageIndex * pageSize).Take(pageSize);
            else if (pageSize > 0)
                target = target.Take(pageSize);

            return target;
        }

        public static IQueryable<TEntity> Search<TEntity, TRelated>(this IQueryable<TEntity> target,
            string navigationProperty,
            IEnumerable<SearchyCondition> conditions,
            IEnumerable<SearchySort> orders = null,
            int pageSize = 0,
            int pageIndex = 0)
        {
            var param = Expression.Parameter(typeof(TEntity), "TEntity");
            var _parammany = Expression.Parameter(typeof(TRelated), "TRelated");
            var _finalexp = _parammany.BuildConditionsExpression<TRelated>(conditions);

            if (_finalexp != null)
            {
                var anyMethond = typeof(Enumerable).GetMethods().Where(m => m.Name == "Any" & m.GetParameters().Length == 2).Single().MakeGenericMethod(typeof(TRelated));
                var innerFunction = Expression.Lambda<Func<TRelated, bool>>(_finalexp, _parammany);
                var finalWherEexp = Expression.Lambda<Func<TEntity, bool>>(Expression.Call(anyMethond, Expression.Property(param, typeof(TEntity).GetProperty(navigationProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)), innerFunction), new ParameterExpression[] { param });
                target = target.Where(finalWherEexp);
            }

            if (orders != null && orders.Count() > 0)
            {
                var mainOrderBy = orders.FirstOrDefault();
                var mainSortMemberType = typeof(TEntity).GetProperty(mainOrderBy.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;

                target = target.BuildOrderByThenBy(mainOrderBy, mainSortMemberType, true);

                var searchySorts = new List<SearchySort>
                {
                    mainOrderBy
                };

                foreach (var searchySort in orders.Except(searchySorts.AsEnumerable()))
                {
                    var sortMemberType = typeof(TEntity).GetProperty(searchySort.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    target = target.BuildOrderByThenBy(searchySort, sortMemberType, false);
                }
            }

            if (pageSize > 0 & pageIndex > 0)
                target = target.Skip(pageIndex * pageSize).Take(pageSize);
            else if (pageSize > 0)
                target = target.Take(pageSize);

            return target;

        }

        static IQueryable<TEntity> BuildOrderBy<U, TEntity>(this IQueryable<TEntity> query, SearchySort searchySort)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "");
            if (searchySort.Sort == SearchySortOrder.ASC)
                return query.OrderBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, searchySort.Field), new ParameterExpression[] { pe }));
            else
                return query.OrderByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, searchySort.Field), new ParameterExpression[] { pe }));
        }

        static IQueryable<TEntity> BuildThenBy<U, TEntity>(this IQueryable<TEntity> query, SearchySort searchySort)
        {
            var pe = Expression.Parameter(typeof(TEntity), "");
            var orderedQuery = (IOrderedQueryable<TEntity>)query;

            if (searchySort.Sort == SearchySortOrder.ASC)
                return orderedQuery.ThenBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, searchySort.Field), new ParameterExpression[] { pe }));
            else
                return orderedQuery.ThenByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, searchySort.Field), new ParameterExpression[] { pe }));
        }

        static IQueryable<TEntity> BuildOrderByThenBy<TEntity>(this IQueryable<TEntity> query, SearchySort searchySort, Type type, bool mainOrderBy)
        {
            switch (true)
            {
                case object _ when type.Equals(typeof(int)):

                    if (mainOrderBy)
                        return query.BuildOrderBy<int, TEntity>(searchySort);
                    else
                        return query.BuildThenBy<int, TEntity>(searchySort);

                case object _ when type.Equals(typeof(string)):

                    if (mainOrderBy)
                        return query.BuildOrderBy<string, TEntity>(searchySort);
                    else
                        return query.BuildThenBy<string, TEntity>(searchySort);

                case object _ when type.Equals(typeof(DateTime?)):

                    if (mainOrderBy)
                        return query.BuildOrderBy<DateTime?, TEntity>(searchySort);
                    else
                        return query.BuildThenBy<DateTime?, TEntity>(searchySort);

                case object _ when type.Equals(typeof(DateTime)):

                    if (mainOrderBy)
                        return query.BuildOrderBy<DateTime, TEntity>(searchySort);
                    else
                        return query.BuildThenBy<DateTime, TEntity>(searchySort);

                case object _ when type.Equals(typeof(byte)):

                    if (mainOrderBy)
                        return query.BuildOrderBy<byte, TEntity>(searchySort);
                    else
                        return query.BuildThenBy<byte, TEntity>(searchySort);

                case object _ when type.Equals(typeof(short)):

                    if (mainOrderBy)
                        return query.BuildOrderBy<short, TEntity>(searchySort);
                    else
                        return query.BuildThenBy<short, TEntity>(searchySort);

                case object _ when type.Equals(typeof(decimal)):

                    if (mainOrderBy)
                        return query.BuildOrderBy<decimal, TEntity>(searchySort);
                    else
                        return query.BuildThenBy<decimal, TEntity>(searchySort);

                default:

                    throw new SWException("Unsupported sort datatype: " + type.ToString());

            }

          

         
        }
    }
}
