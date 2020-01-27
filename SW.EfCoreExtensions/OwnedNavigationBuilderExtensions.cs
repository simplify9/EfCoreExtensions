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
            builder.Property(p => p.Street).IsSeparatorDelimited();
            builder.Property(p => p.Country).HasColumnName($"{prefix}Country").IsCode(2, required: required);
            builder.Property(p => p.City).HasColumnName($"{prefix}City").HasMaxLength(100).IsRequired(required);
            builder.Property(p => p.State).HasColumnName($"{prefix}State").HasMaxLength(100);
            builder.Property(p => p.PostCode).HasColumnName($"{prefix}PostCode").HasMaxLength(20).IsUnicode(false);
            builder.Property(p => p.Street).HasColumnName($"{prefix}Street").HasMaxLength(1024);

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, GeoPosition> BuildGeoPosition<TOwner>(this OwnedNavigationBuilder<TOwner, GeoPosition> builder, string prefix = null)
            where TOwner : class
        {
            builder.Property(p => p.Latitude).HasColumnName($"{prefix}Latitude").IsDecimal_10_8();
            builder.Property(p => p.Longitude).HasColumnName($"{prefix}Longitude").IsDecimal_10_8();

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Contact> BuildContact<TOwner>(this OwnedNavigationBuilder<TOwner, Contact> builder, string prefix = null, bool required = false)
            where TOwner : class
        {
            builder.Property(p => p.Phones).IsSeparatorDelimited();
            builder.Property(p => p.Emails).IsSeparatorDelimited();
            builder.Property(p => p.Name).HasColumnName($"{prefix}Name").HasMaxLength(100).IsRequired(required);
            builder.Property(p => p.Phones).HasColumnName($"{prefix}Phones").HasMaxLength(100);
            builder.Property(p => p.Emails).HasColumnName($"{prefix}Emails").HasMaxLength(1024);

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Dimensions> BuildDimensions<TOwner>(this OwnedNavigationBuilder<TOwner, Dimensions> builder, string prefix = null, bool required = false)
          where TOwner : class
        {
            builder.Property(d => d.Height).IsDecimal_16_6().HasColumnName($"{prefix}Height");
            builder.Property(d => d.Length).IsDecimal_16_6().HasColumnName($"{prefix}Length");
            builder.Property(d => d.Width).IsDecimal_16_6().HasColumnName($"{prefix}Width");
            builder.Property(d => d.Unit).IsUnicode(false).HasMaxLength(2).HasColumnName($"{prefix}DimensionUnit");
            builder.Property(d => d.Unit).HasConversion<string>();

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Weight> BuildWeight<TOwner>(this OwnedNavigationBuilder<TOwner, Weight> builder, string prefix = null, bool required = false)
          where TOwner : class
        {
            builder.Property(d => d.Value).IsDecimal_16_6().HasColumnName($"{prefix}Weight");
            builder.Property(d => d.Unit).IsUnicode(false).HasMaxLength(2).IsFixedLength().HasColumnName($"{prefix}WeightUnit");
            builder.Property(d => d.Unit).HasConversion<string>();

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Money> BuildMoney<TOwner>(this OwnedNavigationBuilder<TOwner, Money> builder, string prefix = null, bool required = false)
         where TOwner : class
        {
            builder.Property(e => e.Amount).IsDecimal_16_6().HasColumnName($"{prefix}Value");
            builder.Property(e => e.Currency).IsCode(3, false).HasColumnName($"{prefix}Currency");

            return builder;
        }

        public static OwnedNavigationBuilder<TOwner, Audit> BuildAudit<TOwner>(this OwnedNavigationBuilder<TOwner, Audit> builder)
            where TOwner : class
        {
            builder.Property(d => d.CreatedBy).IsUnicode(false).HasMaxLength(50).HasColumnName("CreatedBy");
            builder.Property(d => d.ModifiedBy).IsUnicode(false).HasMaxLength(50).HasColumnName("ModifiedBy");
            builder.Property(d => d.CreatedOn).HasColumnName("CreatedOn");
            builder.Property(d => d.ModifiedOn).HasColumnName("ModifiedOn");

            return builder;
        }

    }
}
