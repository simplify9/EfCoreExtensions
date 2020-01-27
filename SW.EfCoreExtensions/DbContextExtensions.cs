using Microsoft.EntityFrameworkCore;
using SW.PrimitiveTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.EfCoreExtensions
{
    public static class DbContextExtensions
    {
        //public static async Task DeleteAsync<TEntity>(this DbContext dbContext, TEntity entity) 
        //    where TEntity : class
        //{
        //    dbContext.Set<TEntity>().Remove(entity);
        //    await dbContext.SaveChangesAsync();
        //}

        //async public static Task DeleteAsync<TEntity>(this DbContext context, object key)
        //    where TEntity : class
        //{
        //    var entity = await context.FindAsync<TEntity>(key);
        //    context.Remove(entity);
        //    await context.SaveChangesAsync();
        //}

        //async public static Task CreateAsync<TEntity>(this DbContext context, TEntity entity)
        //    where TEntity : class
        //{
        //    context.Add(entity);
        //    await context.SaveChangesAsync();
        //    //return entity.Id.ToString();
        //}

        //public static ValueTask<TEntity> GetAsync<TEntity>(this DbContext context, object key)
        //    where TEntity : BaseEntity
        //{
        //    return context.Set<TEntity>().FindAsync(int.Parse(key.ToString()));
        //}

        //public static async Task UpdateAsync<TEntity>(this DbContext dbContext, TEntity entity) 
        //    where TEntity : BaseEntity
        //{
        //    dbContext.Entry(entity).State = EntityState.Modified;
        //    await dbContext.SaveChangesAsync();
        //}

        //async public static Task UpdateAsync<TEntity>(this DbContext context, object key, object dto)
        //    where TEntity : BaseEntity
        //{
        //    var entity = await context.GetAsync<TEntity>(key);
        //    context.Entry(entity).SetProperties(dto);
        //    await context.SaveChangesAsync();
        //}

        //async public static Task CreateOrUpdateAsync<TEntity>(this DbContext context, object key, object dto, TEntity newEntity)
        //    where TEntity : BaseEntity
        //{
        //    var existingEntity = await context.GetAsync<TEntity>(key);

        //    if (existingEntity is null)
        //    {
        //        context.Add(newEntity);
        //    }
        //    else
        //    {
        //        context.Entry(existingEntity).SetProperties(dto);
        //    }
        //    await context.SaveChangesAsync();
        //}

        public static async Task<List<TEntity>> ListAsync<TEntity>(this DbContext dbContext, ISpecification<TEntity> spec = null) where TEntity : BaseEntity
        {
            var query = dbContext.Set<TEntity>().AsQueryable();

            if (spec != null)
            {
                query = query.Where(spec.Criteria);
            }

            return await query.ToListAsync();
        }

        public static List<TEntity> List<TEntity>(this DbContext dbContext, ISpecification<TEntity> spec = null) where TEntity : BaseEntity
        {
            var query = dbContext.Set<TEntity>().AsQueryable();

            if (spec != null)
            {
                query = query.Where(spec.Criteria);
            }

            return query.ToList();
        }


    }
}
