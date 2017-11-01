using Kard.DI;
using Kard.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Core.IRepositories
{
    public interface IRepository : ISingletonService
    {
        bool Create<T>(T entity) where T : class, ICreationAuditedEntity;

        Task<bool> CreateAsync<T>(T entity) where T : class, ICreationAuditedEntity;

        T FirstOrDefault<TKey, T>(TKey id) where T : class;

        Task<T> FirstOrDefaultAsync<TKey, T>(TKey id) where T : class;

        T UniquenessOrDefault<T>(object predicate) where T : class;



        IEnumerable<T> GetList<T>() where T : class;

        Task<IEnumerable<T>> GetListAsync<T>() where T : class;

        IEnumerable<T> GetList<T>(object predicate) where T : class;

        Task<IEnumerable<T>> GetListAsync<T>(object predicate) where T : class;

        IEnumerable<T> GetPage<T>(object predicate, int page, int resultsPerPage) where T : class;

        bool Update<T>(T entity) where T : class, ILastModificationAuditedEntity;

        Task<bool> UpdateAsync<T>(T entity) where T : class, ILastModificationAuditedEntity;

        bool Delete<T>(T entity) where T : class;

        Task<bool> DeleteAsync<T>(T entity) where T : class;
    }
}
