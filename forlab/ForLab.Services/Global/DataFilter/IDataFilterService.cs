using System.Linq;

namespace ForLab.Services.Global.DataFilter
{
    public interface IDataFilterService<T>
    {
        IQueryable<T> Filter(IQueryable<T> data, object filterDto);
    }
}
