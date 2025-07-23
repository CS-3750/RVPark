using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Retrieve a single entity by its primary key ID
        T GetById(int? id);
        
        // Retrieve a single entity by its primary key ID
        T GetById(string? id);

        // Get
        T Get(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null);

        // Async Get
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null);

        // Get All
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null);

        // Async Get All
        // Get All
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null);

        // Insert a new entity
        void Add(T entity);

        // Remove
        void Delete(T entity);

        // Remove multiple
        void Delete(IEnumerable<T> entities);

        // Update existing
        void Update(T entity);
    }
}
