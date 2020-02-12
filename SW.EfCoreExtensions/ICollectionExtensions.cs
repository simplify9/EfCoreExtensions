using System.Collections.Generic;
using System.Linq;

namespace SW.EfCoreExtensions
{
    public static class ICollectionExtensions
    {
        public static void Update<TSource>(this ICollection<TSource> first, IEnumerable<TSource> second)
        {
            first.Except(second).ToList().ForEach(s => first.Remove(s));
            second.Except(first).ToList().ForEach(s => first.Add(s));
        }
    }
}
