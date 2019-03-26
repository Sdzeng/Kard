using Kard.Core.Dtos;
using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Core.IRepositories
{
    public interface IDefaultRepository: IRepository
    {
        #region CRUD

        #region Create
        ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, int? commandTimeout = null) where T : class;

        Task<ResultDto<TKey>> CreateAndGetIdAsync<T, TKey>(T entity, int? commandTimeout = null) where T : class;

        //ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, int? commandTimeout = null) where T : class, ICreationAuditedEntity;

        ResultDto CreateList<T>(IEnumerable<T> entities, int? commandTimeout = null) where T : class;
        #endregion

        #region Retrieve
        IEnumerable<dynamic> Query(string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        IEnumerable<object> Query(Type type, string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        IEnumerable<T> Query<T>(string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));


        T FirstOrDefault<T>(object id, int? commandTimeout = null) where T : class;

        Task<T> FirstOrDefaultAsync<T>(object id, int? commandTimeout = null) where T : class;

        //T UniquenessOrDefault<T>(string sql, IDictionary<string, object> parameters = null, bool serializeParameters = false, IDbConnection connection = null, int? commandTimeout = null);

        T FirstOrDefaultByPredicate<T>(object predicate, int? commandTimeout = null) where T : class;

        int Count(string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?));

        int Count<T>(object predicate, int? commandTimeout = default(int?)) where T : class;

        //int Count(string tableName, IDictionary<string, object> parameters = null, bool serializeParameters = false, int? commandTimeout = null);



        #endregion

        #region Update
        ResultDto Update<T>(T entity, int? commandTimeout = null) where T : class;

        Task<ResultDto> UpdateAsync<T>(T entity, int? commandTimeout = null) where T : class;
        #endregion

        #region Delete
        ResultDto Delete<T>(T entity, int? commandTimeout = null) where T : class;

        Task<ResultDto> DeleteAsync<T>(T entity, int? commandTimeout = null) where T : class;

        ResultDto DeleteList<T>(object predicate, int? commandTimeout = null) where T : class;

        #endregion

 
        #endregion

        ICoverRepository Cover { get; }

        IEssayCommentRepository EssayComment { get; }

        IEssayLikeRepository EssayLike { get; }

        IEssayRepository Essay { get; }


        IKuserRepository Kuser { get; }

        ILongTaskRepository LongTask { get; }

    }
}

  
 