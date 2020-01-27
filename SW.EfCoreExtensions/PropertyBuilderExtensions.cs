using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.EfCoreExtensions
{
    public static class PropertyBuilderExtensions
    {
        //static readonly ValueConverter<string[], string> _stringArrayConverter = 



        /// <summary>
        /// Stores array as a separator delimited string
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static PropertyBuilder<string> IsCode(this PropertyBuilder<string> builder, byte length, bool required = true, bool fixedLength = true)
        {
            builder.IsUnicode(false).IsRequired(required).IsFixedLength(fixedLength).HasMaxLength(length);
            return builder;
        }

        public static PropertyBuilder<decimal> IsDecimal_10_8(this PropertyBuilder<decimal> builder)
        {
            builder.HasColumnType("decimal(10,8)");//IsUnicode(false).IsRequired(Required).IsFixedLength(FixedLength).HasMaxLength(Length);
            return builder;
        }

        public static PropertyBuilder<decimal?> IsDecimal_10_8(this PropertyBuilder<decimal?> builder)
        {
            builder.HasColumnType("decimal(10,8)");//IsUnicode(false).IsRequired(Required).IsFixedLength(FixedLength).HasMaxLength(Length);
            return builder;
        }

        public static PropertyBuilder<decimal> IsDecimal_16_6(this PropertyBuilder<decimal> builder)
        {
            builder.HasColumnType("decimal(16,6)");//IsUnicode(false).IsRequired(Required).IsFixedLength(FixedLength).HasMaxLength(Length);
            return builder;
        }

        public static PropertyBuilder<decimal?> IsDecimal_16_6(this PropertyBuilder<decimal?> builder)
        {
            builder.HasColumnType("decimal(16,6)");//IsUnicode(false).IsRequired(Required).IsFixedLength(FixedLength).HasMaxLength(Length);
            return builder;
        }

        public static PropertyBuilder<int[]> IsSeparatorDelimited(this PropertyBuilder<int[]> builder, char separator = ';')
        {
            var c = new ValueConverter<int[], string>(
            array => array == null || array.Length < 1
                ? null
                : string.Join(separator.ToString(), array),
            str => str == null
                ? new int[] { }
                : str.Split(separator).Select(e=> int.Parse(e)).ToArray());

            builder.HasConversion(c);
            return builder;
        }

        public static PropertyBuilder<string[]> IsSeparatorDelimited(this PropertyBuilder<string[]> builder, char separator = ';')
        {
            var c =new ValueConverter<string[], string>(
            array => array == null || array.Length < 1
                ? null
                : string.Join(separator.ToString(), array),
            str => str == null
                ? new string[] { }
                : str.Split(separator));

            builder.HasConversion(c);
            return builder;
        }

        public static PropertyBuilder<Type> IsClrType(this PropertyBuilder<Type> builder)
        {

            ValueConverter<Type, string> _clrTypeConverter = new ValueConverter<Type, string>(
                type => type == null
                    ? null
                    : type.AssemblyQualifiedName,
                str => str == null
                    ? null
                    : Type.GetType(str));
            builder.HasConversion(_clrTypeConverter);
            return builder;
        }

        /// <summary>
        /// Stores value as a JSON serialized string
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        //public static PropertyBuilder<TProperty> StoreAsJson<TProperty>(this PropertyBuilder<TProperty> builder)
        //    where TProperty : class
        //{
        //    builder.HasConversion(new ValueConverter<TProperty, string>(
        //        obj => obj == null
        //            ? null
        //            : JsonConvert.SerializeObject(obj),
        //        str => str == null
        //            ? null
        //            : JsonConvert.DeserializeObject<TProperty>(str)));

        //    return builder;
        //}

        public static PropertyBuilder<TProperty> StoreAsJson<TProperty>(this PropertyBuilder<TProperty> builder)
        {
            ValueConverter<TProperty, String> converter = new ValueConverter<TProperty, String>(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<TProperty>(v));

            ValueComparer<TProperty> comparer = new ValueComparer<TProperty>(
                (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
                v => v == null ? 0 : JsonConvert.SerializeObject(v).GetHashCode(),
                v => JsonConvert.DeserializeObject<TProperty>(JsonConvert.SerializeObject(v)));

            builder.HasConversion(converter);
            builder.Metadata.SetValueConverter(converter);
            builder.Metadata.SetValueComparer(comparer);

            return builder;
        }
    }
}
