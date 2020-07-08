using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (var entry in changeTracker.Entries())
            {

                if (entry.Entity is ISoftDelete && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(ISoftDelete.Deleted)).CurrentValue = true;

                    if (entry.Entity is IHasDeletionTime)

                        entry.Property(nameof(IHasDeletionTime.DeletedOn)).CurrentValue = timestamp;

                    if (entry.Entity is IDeletionAudited)

                        entry.Property(nameof(IDeletionAudited.DeletedBy)).CurrentValue = requestContext?.GetNameIdentifier();

                }

                if (entry.Entity is IHasCreationTime && entry.State == EntityState.Added)

                    entry.Property(nameof(IHasCreationTime.CreatedOn)).CurrentValue = timestamp;

                if (entry.Entity is ICreationAudited && entry.State == EntityState.Added)

                    entry.Property(nameof(ICreationAudited.CreatedBy)).CurrentValue = requestContext?.GetNameIdentifier();

                if (entry.Entity is IHasModificationTime && (entry.State == EntityState.Added || entry.State == EntityState.Modified))

                    entry.Property(nameof(IHasModificationTime.ModifiedOn)).CurrentValue = timestamp;

                if (entry.Entity is IModificationAudited && (entry.State == EntityState.Added || entry.State == EntityState.Modified))

                    entry.Property(nameof(IModificationAudited.ModifiedBy)).CurrentValue = requestContext?.GetNameIdentifier();

            }
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
