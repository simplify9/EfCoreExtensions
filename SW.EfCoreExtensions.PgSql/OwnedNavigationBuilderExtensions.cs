using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SW.PrimitiveTypes;


namespace SW.EfCoreExtensions
{
    public static class OwnedNavigationBuilderExtensions
    {

        public static OwnedNavigationBuilder<TOwner, StreetAddress> BuildStreetAddress<TOwner>(this OwnedNavigationBuilder<TOwner, StreetAddress> builder, string prefix = null, bool required = false)
            where TOwner : class
        {
            //builder.Property(p => p.Street).IsSeparatorDelimited();
            builder.Property(p => p.Country).HasColumnName($"{prefix}country").IsCode(2, required: required);
            builder.Property(p => p.City).HasColumnName($"{prefix}city").HasMaxLength(100).IsRequired(required);
            builder.Property(p => p.State).HasColumnName($"{prefix}state").HasMaxLength(100);
            builder.Property(p => p.PostCode).HasColumnName($"{prefix}post_code").HasMaxLength(20).IsUnicode(false);
            builder.Property(p => p.Street).HasColumnName($"{prefix}street").HasMaxLength(1024);

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, GeoPosition> BuildGeoPosition<TOwner>(this OwnedNavigationBuilder<TOwner, GeoPosition> builder, string prefix = null)
            where TOwner : class
        {
            builder.Property(p => p.Latitude).HasColumnName($"{prefix}latitude").HasColumnType("decimal(11,8)");
            builder.Property(p => p.Longitude).HasColumnName($"{prefix}longitude").HasColumnType("decimal(11,8)");

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Contact> BuildContact<TOwner>(this OwnedNavigationBuilder<TOwner, Contact> builder, string prefix = null, bool required = false)
            where TOwner : class
        {
            //builder.Property(p => p.Phones).IsSeparatorDelimited();
            //builder.Property(p => p.Emails).IsSeparatorDelimited();
            builder.Property(p => p.Name).HasColumnName($"{prefix}name").HasMaxLength(100).IsRequired(required);
            builder.Property(p => p.Phones).HasColumnName($"{prefix}phones").HasMaxLength(100);
            builder.Property(p => p.Emails).HasColumnName($"{prefix}emails").HasMaxLength(1024);

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Dimensions> BuildDimensions<TOwner>(this OwnedNavigationBuilder<TOwner, Dimensions> builder, string prefix = null, bool required = false)
          where TOwner : class
        {
            builder.Property(d => d.Height).HasColumnType("decimal(16,6)").HasColumnName($"{prefix}height");
            builder.Property(d => d.Length).HasColumnType("decimal(16,6)").HasColumnName($"{prefix}length");
            builder.Property(d => d.Width).HasColumnType("decimal(16,6)").HasColumnName($"{prefix}width");
            builder.Property(d => d.Unit).IsUnicode(false).HasMaxLength(2).HasColumnName($"{prefix}dimension_unit");
            builder.Property(d => d.Unit).HasConversion<string>();

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Weight> BuildWeight<TOwner>(this OwnedNavigationBuilder<TOwner, Weight> builder, string prefix = null, bool required = false)
          where TOwner : class
        {
            builder.Property(d => d.Value).HasColumnType("decimal(16,6)").HasColumnName($"{prefix}weight");
            builder.Property(d => d.Unit).IsUnicode(false).HasMaxLength(2).IsFixedLength().HasColumnName($"{prefix}weight_unit");
            builder.Property(d => d.Unit).HasConversion<string>();

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Money> BuildMoney<TOwner>(this OwnedNavigationBuilder<TOwner, Money> builder, string prefix = null, bool required = false)
         where TOwner : class
        {
            builder.Property(e => e.Amount).HasColumnType("decimal(16,6)").HasColumnName($"{prefix}value");
            builder.Property(e => e.Currency).IsCode(3, false).HasColumnName($"{prefix}currency");

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Audit> BuildAudit<TOwner>(this OwnedNavigationBuilder<TOwner, Audit> builder)
            where TOwner : class
        {
            builder.Property(d => d.CreatedBy).IsUnicode(false).HasMaxLength(50).HasColumnName("created_by");
            builder.Property(d => d.ModifiedBy).IsUnicode(false).HasMaxLength(50).HasColumnName("modified_by");
            builder.Property(d => d.CreatedOn).HasColumnName("created_on");
            builder.Property(d => d.ModifiedOn).HasColumnName("modified_on");

            return builder;
        }

    }
}
