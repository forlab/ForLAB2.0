using System.Threading.Tasks;

namespace ForLab.Repositories.UOW
{
    public interface IUnitOfWork<T>
    {
        int Commit();
        Task<int> CommitAsync();
    }
}
