using Dapper;
using DapperExtensionsCore;
using Kard.Core.Dtos;
using Kard.Domain.Entities.Auditing;
using Kard.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    /// <summary>
    /// Repository使用的CRUD
    /// </summary>
    public static class DbConnectionExtensions
    {



        public static ResultDto<TKey> CreateAndGetId<T, TKey>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var result = new ResultDto<TKey>();
            TKey id = connection.Insert<T>(entity, transaction, commandTimeout);
            result.Result = (!id.Equals(default(TKey)));
            if (!result.Result)
            {
                result.Message = "新增失败";
            }
            else
            {
                result.Message = "新增成功";
                result.Data = id;
            }
            return result;
        }


        public static ResultDto CreateList<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var result = new ResultDto { Result = true };

            try
            {
                if (entities != null && entities.Any())
                {
                    connection.Insert(entities, transaction, commandTimeout);
                }
                else
                {
                    result.Result = false;
                    result.Message = "列表为空";
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public static dynamic QueryList(this IDbConnection connection, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {
            return connection.Query(sql, parameters, transaction, true, commandTimeout, commandType);
        }

        public static IEnumerable<object> QueryList(this IDbConnection connection, Type type, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {
            return connection.Query(type, sql, parameters, transaction, true, commandTimeout, commandType);
        }

        //parameters  IDictionary<string, object>,new {}
        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {

            return connection.Query<T>(sql, parameters, transaction, true, commandTimeout, commandType);
        }

        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return connection.GetList<T>(predicate, sort, transaction, commandTimeout, true);
        }




        public static T FirstOrDefault<T, TKey>(this IDbConnection connection, TKey id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return connection.Get<T>(id, transaction, commandTimeout);
        }

        public static T FirstOrDefault<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var entityList = connection.GetList<T>(predicate, null, transaction, commandTimeout);
            if (entityList?.Count() != 1)
            {
                return default(T);
            }

            return entityList.First();
        }

        public static int Count(this IDbConnection connection, string sql, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {
            return connection.ExecuteScalar<int>(sql, parameters, transaction, commandTimeout, commandType);
        }


        public static ResultDto UpdateList<T>(this IDbConnection connection, IEnumerable<T> entityList, IDbTransaction transaction, int? commandTimeout = null) where T : class
        {
            var resultDto = new ResultDto();
            foreach (var entity in entityList)
            {
                if (!connection.Update(entity, transaction, commandTimeout))
                {
                    resultDto.Result = false;
                    resultDto.Message = $"更新{nameof(T)}失败 entity:{Serialize.ToJson(entity)}";
                    return resultDto;
                }
            }
            resultDto.Result = true;
            return resultDto;
        }

    }
}
