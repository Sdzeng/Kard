using DapperExtensions;
using Kard.Core.Dtos;
using Kard.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Dapper.SqlMapper;

namespace Kard.Dapper.Mysql
{

    public static class DapperExtensions
    {
        public static IEnumerable<TFirst> Map<TFirst, TSecond, TKey>(this GridReader reader, Func<TFirst, TKey> firstKey, Func<TSecond, TKey> secondKey, Action<TFirst, IEnumerable<TSecond>> addChildren)
        {
            var first = reader.Read<TFirst>().ToList();
            var childMap = reader
                .Read<TSecond>()
                .GroupBy(s => secondKey(s))
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            foreach (var item in first)
            {
                IEnumerable<TSecond> children;
                if (childMap.TryGetValue(firstKey(item), out children))
                {
                    addChildren(item, children);
                }
            }

            return first;
        }


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

        public static T FirstOrDefaultByPredicate<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var entityList = connection.GetList<T>(predicate, null, transaction, commandTimeout);
            if (entityList?.Count() != 1)
            {
                return default(T);
            }

            return entityList.First();
        }
    }

}
