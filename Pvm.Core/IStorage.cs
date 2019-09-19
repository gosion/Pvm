using System;
using System.Threading.Tasks;

namespace Pvm.Core
{
    public interface IStorage<T>
    {
        Task<T> GetSync(Guid id);
        Task AddAsync(T model);
        Task SaveAsync(T model);
        Task RemoveAsync(T model);
    }
}
