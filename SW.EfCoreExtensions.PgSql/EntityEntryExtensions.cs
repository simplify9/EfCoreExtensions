using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System.Reflection;

namespace SW.EfCoreExtensions
{
    public static class EntityEntryExtensions
    {
        public static void SetProperties(this EntityEntry entityEntry, object dto)
        {
            var props = entityEntry.CurrentValues.Properties.Where(prop => prop.PropertyInfo != null).Select(prop => prop.PropertyInfo);

            foreach (var propInfo in props)
                if (propInfo.GetSetMethod() != null)
                {
                    var propInfoSource = dto.GetType().GetProperty(propInfo.Name, BindingFlags.Public | BindingFlags.Instance);

                    if (propInfoSource != null && propInfoSource.GetGetMethod() != null)
                        propInfo.SetValue(entityEntry.Entity, propInfoSource.GetValue(dto));
                }
        }
    }
}
