using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SW.PrimitiveTypes;

namespace SW.EfCoreExtensions
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> HasQueryFilterForSoftDeletion<T>(this EntityTypeBuilder<T> builder) where T : class, ISoftDelete
            => builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)));

        public static EntityTypeBuilder<T> HasQueryFilterForTenancy<T>(this EntityTypeBuilder<T> builder, RequestContext requestContext) where T : class, IHasTenant
            => builder.HasQueryFilter(item => EF.Property<int>(item, nameof(IHasTenant.TenantId)) == requestContext.GetTenantId().Value);

        public static EntityTypeBuilder<T> HasQueryFilterForOptionalTenancy<T>(this EntityTypeBuilder<T> builder, RequestContext requestContext) where T : class, IHasOptionalTenant
            => builder.HasQueryFilter(item => EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == requestContext.GetTenantId() || EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == null);

        public static EntityTypeBuilder<T> HasQueryFilterForSoftDeletionAndTenancy<T>(this EntityTypeBuilder<T> builder, RequestContext requestContext) where T : class, ISoftDelete, IHasTenant
            => builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)) && EF.Property<int>(item, nameof(IHasTenant.TenantId)) == requestContext.GetTenantId().Value);

        public static EntityTypeBuilder<T> HasQueryFilterForSoftDeletionAndOptionalTenancy<T>(this EntityTypeBuilder<T> builder, RequestContext requestContext) where T : class, ISoftDelete, IHasOptionalTenant
            => builder.HasQueryFilter(item => !EF.Property<bool>(item, nameof(ISoftDelete.Deleted)) && (EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == requestContext.GetTenantId() || EF.Property<int?>(item, nameof(IHasOptionalTenant.TenantId)) == null));
    }
}
