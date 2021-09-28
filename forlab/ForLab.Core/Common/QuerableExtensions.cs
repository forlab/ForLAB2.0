using System.ComponentModel;
using System.Linq;

namespace ForLab.Core.Common
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QuerableExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            int totalCount = query.Count();
            IQueryable<T> collection = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new PaginatedList<T>(collection, pageIndex, pageSize, totalCount);
        }
    }
}
