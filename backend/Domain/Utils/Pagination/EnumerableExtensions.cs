using System.Collections.Generic;
using System.Linq;

namespace backend.Domain.Utils.Pagination
{
    public static class EnumerableExtensions
    {
        public static PagedResponse<T> ToPagedResponse<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var totalCount = source.Count();
            var data = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            return new PagedResponse<T>(data, totalCount, pageNumber, pageSize);
        }
    }
}
