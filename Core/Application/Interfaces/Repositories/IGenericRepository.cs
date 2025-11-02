

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        // Ekstra metotlar
        Task AddRangeAsync(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);
        // IQueryable döndürerek ReportService'in üzerine .Where().ToListAsync() yazmasını sağlıyoruz.
        IQueryable<T> GetAll();
       
        Task<int> CountAsync();

        // Opsiyonel olarak filtreli sayım
        Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

    }
}