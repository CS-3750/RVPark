using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using RVPark.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RVPark.Application
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly ApplicationDbContext _db;

        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
            _db.SaveChanges();
        }

        public void Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
            _db.SaveChanges();
        }

        public void Delete(IEnumerable<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
            _db.SaveChanges();
        }

        public T Get(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null)
        {
            return BuildQuery(predicate, trackChanges, includes).FirstOrDefault() ?? throw new InvalidOperationException($"Entity of type {typeof(T)} matching the predicate was not found.");
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null)
        {
            var query = BuildQuery(predicate ?? (_ => true), trackChanges: false, includes);
            if (orderBy != null) {  query = query.OrderBy(orderBy); }
            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null)
        {
            var query = BuildQuery(predicate ?? (_ => true), trackChanges: false, includes);
            if (orderBy != null) { query = query.OrderBy(orderBy); }
            var result = await query.ToListAsync();
            return result.AsEnumerable();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null)
        {
            return await BuildQuery(predicate, trackChanges, includes).FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Entity of type {typeof(T)} matching the predicate was not found.");
        }

        public T GetById(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var entity = _db.Set<T>().Find(id.Value);
            return entity ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID {id} was not found.");
        }

        public void Update(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            _db.SaveChanges();
        }

        private IQueryable<T> BuildQuery(Expression<Func<T, bool>> predicate, bool trackChanges, string? includes)
        {
            IQueryable<T> query = _db.Set<T>();
            if(!trackChanges)
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }
    }
}
