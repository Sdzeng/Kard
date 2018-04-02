using DapperExtensionsCore;
using Kard.Core.Dtos;
using Kard.DI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Kard.Core.IRepositories
{

    public interface IRepository : ISingletonService
    {
        IDatabase GetDb();

        IDbConnection GetConnection();

        TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate);

        TResult TransExecute<TResult>(Func<IDbConnection, IDbTransaction, TResult> predicate);

        TResult DbExecute<TResult>(Func<IDatabase, TResult> predicate);

        #region CRUD

        #region Create
        ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        Task<ResultDto<TKey>> CreateAndGetIdAsync<T, TKey>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        //ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, ICreationAuditedEntity;

        bool Create<T>(IEnumerable<T> entities, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        #endregion

        #region Retrieve
        IEnumerable<dynamic> Query(string sql, object parameters = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        IEnumerable<object> Query(Type type, string sql, object parameters = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        IEnumerable<T> Query<T>(string sql, object parameters = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        T FirstOrDefault<T, TKey>(TKey id, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        Task<T> FirstOrDefaultAsync<T, TKey>(TKey id, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        //T UniquenessOrDefault<T>(string sql, IDictionary<string, object> parameters = null, bool serializeParameters = false, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null);

        T FirstOrDefault<T>(object predicate, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        int Count(string sql, object parameters = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        int Count<T>(object predicate, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = default(int?)) where T : class;

        int Count(string tableName, IDictionary<string, object> parameters = null, bool serializeParameters = false, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null);

        IEnumerable<T> GetList<T>(object predicate = null, IList<ISort> sort = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;


        Task<IEnumerable<T>> GetListAsync<T>(object predicate = null, IList<ISort> sort = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;


     
        #endregion

        #region Update
        bool Update<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        Task<bool> UpdateAsync<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        #endregion

        #region Delete
        bool Delete<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        Task<bool> DeleteAsync<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        bool Delete<T>(object predicate, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        #endregion

        string SerializeParameters(IDictionary<string, object> parameters = null);
        #endregion
    }
}
