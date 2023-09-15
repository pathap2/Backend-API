using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TodoList.Api.Repository
{
    /// <summary>
    /// Interface created so that it will hide the actual data interaction from the cotroller method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="DTO"></typeparam>
    public interface IRepository<T, DTO> where T : class
    {
        Task<IEnumerable<DTO>> GetAllAsync(Expression<Func<T, bool>> filter = null);
        Task<DTO> GetByIdAsync(Guid id);
        Task AddAsync(DTO entity);
        Task UpdateAsync(DTO entity);
        Task DeleteAsync(Guid id);
    }
}
