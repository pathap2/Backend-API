using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TodoList.Api.Repository
{
    public interface IRepository<T, DTO> where T : class
    {
        Task<IEnumerable<DTO>> GetAllAsync(Expression<Func<T, bool>> filter = null);
        Task<DTO> GetByIdAsync(Guid id);
        Task AddAsync(DTO entity);
        Task UpdateAsync(DTO entity);
        Task DeleteAsync(Guid id);
    }
}
