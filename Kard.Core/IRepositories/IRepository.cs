using Kard.Core.Dtos;
using Kard.DI;
using System;
using System.Collections.Generic;
using System.Data;

namespace Kard.Core.IRepositories
{

    public interface IRepository : ISingletonService
    {
        //IDatabase GetDb();

        IDbConnection GetConnection();

        TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate);

        TResult TransExecute<TResult>(Func<IDbConnection, IDbTransaction, TResult> predicate);

        //TResult DbExecute<TResult>(Func<IDatabase, TResult> predicate);

        #region CRUD

        #region Create
        //ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;


        //bool Create<T>(IEnumerable<T> entities, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        #endregion

        #region Retrieve

        IEnumerable<T> Query<T>(string sql, object parameters = null,  IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        //T FirstOrDefault<TKey, T>(TKey id, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        //T UniquenessOrDefault<T>(object predicate, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        long GetCount(string tableName, IDictionary<string, object> parameters = null, bool serializeParameters = false, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null);

        //IEnumerable<T> GetList<T>(object predicate = null, IList<ISort> sort = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;


      

        #endregion

        #region Update
        //bool Update<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

       
        #endregion

        #region Delete
        //bool Delete<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

       

        //bool Delete<T>(object predicate, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        #endregion

     
        #endregion
    }
}
