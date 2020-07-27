using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SW.EfCoreExtensions;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SW.EfCoreExtensions
{
    static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> HasQueryFilter<T>(this EntityTypeBuilder<T> builder, QueryFilter queryFilter, RequestContext requestContext = null) where T : class
        {
            switch (queryFilter)
            {
                case QueryFilter.SoftDeletion:
                    if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                        return builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)));
                    
                    break;

                case QueryFilter.Tenancy:
                    if (typeof(IHasTenant).IsAssignableFrom(typeof(T)))
                        return builder.HasQueryFilter(item => EF.Property<int>(item, nameof(IHasTenant.TenantId)) == requestContext.GetTenantId().Value);
                    
                    else if (typeof(IHasOptionalTenant).IsAssignableFrom(typeof(T)))
                        return builder.HasQueryFilter(item => EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == requestContext.GetTenantId() || EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == null);
                    
                    break;

                case QueryFilter.SoftDeletion | QueryFilter.Tenancy:
                    if (typeof(IHasTenant).IsAssignableFrom(typeof(T)) && typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                        return builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)) && EF.Property<int>(item, nameof(IHasTenant.TenantId)) == requestContext.GetTenantId().Value);

                    else if (typeof(IHasOptionalTenant).IsAssignableFrom(typeof(T)) && typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                        return builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)) && (EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == requestContext.GetTenantId() || EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == null));
                    
                    //else if (typeof(IHasTenant).IsAssignableFrom(typeof(T)))
                    //    return builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)));
                    
                    //else if (typeof(IHasOptionalTenant).IsAssignableFrom(typeof(T)))
                    //    return builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)));
                    
                    //else if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                    //    return builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)));
                    
                    break;

            }

            return builder;        
        }
    }
}
