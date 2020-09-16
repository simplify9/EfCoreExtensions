using SW.PrimitiveTypes;
using System;
using System.Reflection;

namespace SW.EfCoreExtensions 
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string columnName)
        {
            ColumnName  = columnName;
        }

        public string ColumnName { get; set; }

        public string ColumnNameEscaped(RelationalDbType relationalDbType) => 
            relationalDbType == RelationalDbType.MySql  ? @$"`{ColumnName}`" :  @$"""{ColumnName}""";

        public static ColumnAttribute Get(PropertyInfo propertyInfo)
        {
            var columnInfo = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            return columnInfo ?? new ColumnAttribute(propertyInfo.Name);
        }

        public static ColumnAttribute Get<TEntity>(string propertyName)
        {
            var property = typeof(TEntity).GetProperty(propertyName);
            if (property == null) throw new SWException(propertyName);
            return Get(property);
        }
    }
}



