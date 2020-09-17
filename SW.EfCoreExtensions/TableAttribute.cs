using System;
using System.Reflection;

namespace SW.EfCoreExtensions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string tableName, string identityColumn = "Id", bool serverSideIdentity = true)
        {
            TableName = tableName;
            IdentityColumn = identityColumn;
            ServerSideIdentity = serverSideIdentity;
        }

        public string TableName { get; set; }
        public string IdentityColumn { get; set; }
        public bool ServerSideIdentity { get; set; }

        public static TableAttribute Get(Type entityType)
        {
            var tableInfo = entityType.GetCustomAttribute<TableAttribute>();
            return tableInfo ?? new TableAttribute(entityType.Name);
        }

        public static TableAttribute Get<TEntity>()
        {
            return Get(typeof(TEntity));
        }
    }
}





