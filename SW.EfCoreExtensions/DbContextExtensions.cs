using Microsoft.EntityFrameworkCore;
using SW.PrimitiveTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.EfCoreExtensions
{
    public static class DbContextExtensions
    {
        async public static Task DeleteByKeyAsync<TEntity>(this DbContext dbContext, object key) where TEntity : class
        {
            var entity = await dbContext.FindAsync<TEntity>(key);
            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public static async Task<List<TEntity>> ListAsync<TEntity>(this DbContext dbContext, ISpecification<TEntity> spec = null) where TEntity : class
        {
            var query = dbContext.Set<TEntity>().AsQueryable();
            if (spec != null)
                query = query.Where(spec.Criteria);
            return await query.ToListAsync();
        }

        public static List<TEntity> List<TEntity>(this DbContext dbContext, ISpecification<TEntity> spec = null) where TEntity : class
        {
            var query = dbContext.Set<TEntity>().AsQueryable();
            if (spec != null)
                query = query.Where(spec.Criteria);
            return query.ToList();
        }

        public static RelationalDbType GetDbType(this DbContext dbContext)
        {
            switch (dbContext.Database.ProviderName.ToLower())
            {
                case "microsoft.entityframeworkcore.sqlserver":
                    return RelationalDbType.MsSql;

                case "pomelo.entityframeworkcore.mysql":
                    return RelationalDbType.MySql;

                case "microsoft.entityframeworkcore.sqlite":
                    return RelationalDbType.Sqlite;

                case "mpgsql.entityframeworkcore.postgresql":
                    return RelationalDbType.Postgre;

                case "oracle.entityframeworkcore":
                    return RelationalDbType.Oracle;

                default:
                    return RelationalDbType.Unknown;
            }
        }
    }
}
