using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SW.EfCoreExtensions
{
    public static class ChangeTrackerExtensions
    {
        public static void SetCommonProperties(this ChangeTracker changeTracker, RequestContext requestContext)
        {
            changeTracker.DetectChanges();

            var timestamp = DateTime.UtcNow;
            var nameIdentifier = requestContext?.GetNameIdentifier();
            var tenantId = requestContext?.GetTenantId();

            foreach (var entry in changeTracker.Entries())
            {

                if (entry.Entity is ISoftDelete && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    TrySetProperty(entry.Entity, nameof(ISoftDelete.Deleted), true);

                    if (entry.Entity is IHasDeletionTime)
                        TrySetProperty(entry.Entity, nameof(IHasDeletionTime.DeletedOn), timestamp);

                    if (entry.Entity is IDeletionAudited)
                        TrySetProperty(entry.Entity, nameof(IDeletionAudited.DeletedBy), nameIdentifier);
                }

                if (entry.Entity is IHasCreationTime && entry.State == EntityState.Added)
                    TrySetProperty(entry.Entity, nameof(IHasCreationTime.CreatedOn), timestamp);

                if (entry.Entity is ICreationAudited && entry.State == EntityState.Added)
                    TrySetProperty(entry.Entity, nameof(ICreationAudited.CreatedBy), nameIdentifier);

                if (entry.Entity is IHasModificationTime && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                    TrySetProperty(entry.Entity, nameof(IHasModificationTime.ModifiedOn), timestamp);

                if (entry.Entity is IModificationAudited && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                    TrySetProperty(entry.Entity, nameof(IModificationAudited.ModifiedBy), nameIdentifier);
            }
        }

        private static void TrySetProperty(object entity, string propertyName, object value)
        {
            var prop = entity.GetType().GetProperty(propertyName);
            var setMethod = prop.GetSetMethod() ?? prop.GetSetMethod(true);
            if (setMethod != null) setMethod.Invoke(entity, new object[] { value });
        }

        async public static Task DispatchDomainEvents(this ChangeTracker changeTracker, IDomainEventDispatcher domainEventDispatcher)
        {
            var entitiesWithEvents = changeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    await domainEventDispatcher.Dispatch(domainEvent);
                }
            }
        }
    }
}
