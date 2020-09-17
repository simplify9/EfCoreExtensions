using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SW.EfCoreExtensions
{
    internal static class DbDataReaderExtensions
    {
        async public static Task<IEnumerable<TEntity>> Bind<TEntity>(this DbDataReader reader) where TEntity : new()
        {
            var properties = typeof(TEntity).GetProperties();
            var list = new List<TEntity>();

            var propertyMapper = new Dictionary<int, int>();

            for (var fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)

                for (var propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
                {
                    string columnName = ColumnAttribute.Get(properties[propertyIndex]).ColumnName;

                    if (columnName.Equals(reader.GetName(fieldIndex), StringComparison.OrdinalIgnoreCase))
                    {
                        propertyMapper.Add(fieldIndex, propertyIndex);
                        break;
                    }
                }


            while (await reader.ReadAsync())
            {
                var entity = new TEntity();

                for (var index = 0; index < reader.FieldCount; index++)
                {

                    if (!reader.IsDBNull(index) && propertyMapper.TryGetValue(index, out int propertyIndex))
                        properties[propertyIndex].SetValue(
                            entity,
                            reader[index].ConvertValueToType(properties[propertyIndex].PropertyType),
                            null
                        );
                }

                list.Add(entity);
            }

            reader.Close();
            return list;
        }

    }
}
