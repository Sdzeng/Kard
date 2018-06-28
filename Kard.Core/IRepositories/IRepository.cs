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
        //IDatabase GetDb();

        //IDbConnection GetConnection();

        //TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate);

        //TResult TransExecute<TResult>(Func<IDbConnection, IDbTransaction, TResult> predicate, IsolationLevel? isolationLevel = null);

        //TResult DbExecute<TResult>(Func<IDatabase, TResult> predicate);

        #region CRUD

        #region Create
        ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, int? commandTimeout = null) where T : class;

        Task<ResultDto<TKey>> CreateAndGetIdAsync<T, TKey>(T entity, int? commandTimeout = null) where T : class;

        //ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, int? commandTimeout = null) where T : class, ICreationAuditedEntity;

        ResultDto CreateList<T>(IEnumerable<T> entities, int? commandTimeout = null) where T : class;
        #endregion

        #region Retrieve
        IEnumerable<dynamic> QueryList(string sql, object parameters = null,  int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        IEnumerable<object> QueryList(Type type, string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        IEnumerable<T> QueryList<T>(string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        IEnumerable<T> QueryList<T>(object predicate = null, IList<ISort> sort = null, int? commandTimeout = null) where T : class;


        Task<IEnumerable<T>> QueryListAsync<T>(object predicate = null, IList<ISort> sort = null,  int? commandTimeout = null) where T : class;



        T FirstOrDefault<T, TKey>(TKey id, int? commandTimeout = null) where T : class;

        Task<T> FirstOrDefaultAsync<T, TKey>(TKey id, int? commandTimeout = null) where T : class;

        //T UniquenessOrDefault<T>(string sql, IDictionary<string, object> parameters = null, bool serializeParameters = false, IDbConnection connection = null, int? commandTimeout = null);

        T FirstOrDefault<T>(object predicate, int? commandTimeout = null) where T : class;

        int Count(string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        int Count<T>(object predicate,  int? commandTimeout = default(int?)) where T : class;

        //int Count(string tableName, IDictionary<string, object> parameters = null, bool serializeParameters = false, int? commandTimeout = null);



        #endregion

        #region Update
        ResultDto Update<T>(T entity, int? commandTimeout = null) where T : class;

        Task<ResultDto> UpdateAsync<T>(T entity,  int? commandTimeout = null) where T : class;
        #endregion

        #region Delete
        ResultDto Delete<T>(T entity, int? commandTimeout = null) where T : class;

        Task<ResultDto> DeleteAsync<T>(T entity,  int? commandTimeout = null) where T : class;

        ResultDto DeleteList<T>(object predicate, int? commandTimeout = null) where T : class;

        #endregion

        //string SerializeParameters(IDictionary<string, object> parameters = null);
        #endregion
    }
}
