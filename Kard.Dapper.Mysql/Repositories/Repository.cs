using Dapper;
using DapperExtensionsCore;
using DapperExtensionsCore.Sql;
using Kard.Core.Dtos;
using Kard.Core.IRepositories;
using Kard.Core.Mappers;
using Kard.Domain.Entities.Auditing;
using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Dapper.Mysql.Repositories
{
    /// <summary>
    /// Web使用的CRUD
    /// </summary>
    public abstract class Repository : IRepository
    {
        private static IDapperExtensionsConfiguration _dapperConfig;
        private static IDapperImplementor _dapperInstance;
        private static IDapperAsyncImplementor _dapperAsyncInstance;
        private static string _connectionString;
        protected readonly ILogger _logger;
        protected readonly EventId _eventId = new EventId(1, "Repository");




        static Repository()
        {
            //1
            //DapperExtensions.Core20.DapperExtensions.DefaultMapper = typeof(CustomPluralizedAutoClassMapper<>);
            //DapperExtensions.Core20.DapperExtensions.SqlDialect = new MySqlDialect();
            //DapperExtensions.DapperExtensions.Configure(
            //    typeof(CustomPluralizedAutoClassMapper<>),
            //    new List<Assembly>(),
            //    new MySqlDialect());

            _dapperConfig = new DapperExtensionsConfiguration(typeof(CustomPluralizedAutoClassMapper<>), new List<Assembly>(), new MySqlDialect());
            //使用connection
            DapperExtensionsCore.DapperExtensions.Configure(_dapperConfig);
            DapperExtensionsCore.DapperAsyncExtensions.Configure(_dapperConfig);
            //使用instance
            _dapperInstance = DapperExtensionsCore.DapperExtensions.InstanceFactory.Invoke(_dapperConfig);
            _dapperAsyncInstance = DapperExtensionsCore.DapperAsyncExtensions.InstanceFactory.Invoke(_dapperConfig);
        }





        public Repository( IConfiguration configuration, ILogger<Repository> logger)
        {
          
            Configuration = configuration;
            _logger = logger;
        }

        protected IConfiguration Configuration { get; }

        protected string ConnectionString
        {
            get
            {
                if (_connectionString.IsNullOrEmpty())
                {
                    _connectionString = Configuration.GetValue<string>("Db:ConnectionString");
                }

                return _connectionString;
            }
        }



        //protected IKardSession KardSession { get; set; }


        protected IDatabase GetDb()
        {
            var connection = GetConnection();
            var sqlGenerator = new SqlGeneratorImpl(_dapperConfig);
            IDatabase _db = new Database(connection, sqlGenerator);
            return _db;
        }

        protected IDbConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }


        //protected PredicateDto GetPredicateDto<T>(object predicate) where T : class
        //{
        //    var p = GetPredicate<T>(predicate);
        //    IDictionary<string, object> parameters = new Dictionary<string, object>();
        //    var sql = p.GetSql(_dapperInstance.SqlGenerator, parameters);
        //    //DynamicParameters dynamicParameters = new DynamicParameters();
        //    //foreach (var parameter in parameters)
        //    //{
        //    //    dynamicParameters.Add(parameter.Key, parameter.Value);
        //    //}
        //    return (new PredicateDto { Sql = sql, Parameters = parameters });
        //}

        //protected IPredicate GetPredicate<T>(object predicate) where T : class
        //{
        //    IClassMapper classMap = _dapperInstance.SqlGenerator.Configuration.GetMap<T>();
        //    IPredicate wherePredicate = predicate as IPredicate;
        //    if (wherePredicate == null && predicate != null)
        //    {
        //        wherePredicate = GetEntityPredicate(classMap, predicate);
        //    }

        //    return wherePredicate;
        //}

        //protected IPredicate GetEntityPredicate(IClassMapper classMap, object entity)
        //{
        //    Type predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);
        //    IList<IPredicate> predicates = new List<IPredicate>();
        //    foreach (var kvp in ReflectionHelper.GetObjectValues(entity))
        //    {
        //        IFieldPredicate fieldPredicate = Activator.CreateInstance(predicateType) as IFieldPredicate;
        //        fieldPredicate.Not = false;
        //        fieldPredicate.Operator = Operator.Eq;
        //        fieldPredicate.PropertyName = kvp.Key;
        //        fieldPredicate.Value = kvp.Value;
        //        predicates.Add(fieldPredicate);
        //    }

        //    return predicates.Count == 1
        //               ? predicates[0]
        //               : new PredicateGroup
        //               {
        //                   Operator = GroupOperator.And,
        //                   Predicates = predicates
        //               };
        //}

        #region Execute

        protected TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate)
        {
            var result = default(TResult);
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    result = predicate.Invoke(connection);
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, string.Empty, result);
                result = default(TResult);
            }


            return result;
        }



        protected TResult TransExecute<TResult>(Func<IDbConnection, IDbTransaction, TResult> predicate, IsolationLevel? isolationLevel = null)
        {
            TResult result;
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    using (IDbTransaction transaction = (isolationLevel.HasValue ? connection.BeginTransaction(isolationLevel.Value) : connection.BeginTransaction()))
                    {
                        try
                        {
                            result = predicate.Invoke(connection, transaction);
                            if (typeof(TResult) == typeof(bool) && Convert.ToBoolean(result) == false)
                            {
                                transaction.Rollback();
                                return result;
                            }

                            if (typeof(TResult) == typeof(ResultDto))
                            {
                                var resultDto = result as ResultDto;
                                if (resultDto.Result == false)
                                {
                                    transaction.Rollback();
                                    return result;
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception e1)
                        {
                            transaction.Rollback();
                            throw e1;
                        }
                        finally
                        {
                            if (connection.State == ConnectionState.Open)
                                connection.Close();
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                _logger.LogError($"TransExecute异常：{e2.Message}");
                result = default(TResult);
            }

            return result;
        }

        protected TResult DbExecute<TResult>(Func<IDatabase, TResult> predicate)
        {
            using (IDatabase db = GetDb())
            {
                var result = predicate.Invoke(db);
                return result;
            }
        }


        #endregion

        #region CRUD

        #region Create
        public ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, int? commandTimeout = null) where T : class
        {
            //if (typeof(ICreationAuditedEntity).IsAssignableFrom(typeof(T)))
            //{
            //    var creationAuditedEntity = entity as ICreationAuditedEntity;
            //    creationAuditedEntity.CreatorUserId = KardSession.UserId;
            //    creationAuditedEntity.CreationTime = DateTime.Now;
            //    entity = creationAuditedEntity as T;
            //}

            return ConnExecute(conn => conn.CreateAndGetId<T, TKey>(entity,null, commandTimeout));
        }



        public Task<ResultDto<TKey>> CreateAndGetIdAsync<T, TKey>(T entity, int? commandTimeout = null) where T : class
        {
            return Task.FromResult(CreateAndGetId<T, TKey>(entity, commandTimeout));
        }

        public ResultDto CreateList<T>(IEnumerable<T> entities,  int? commandTimeout = null) where T : class
        {
           
            //if (entities == null || (!entities.Any()))
            //{
            //    result.Result = false;
            //    result.Message = "列表为空";
            //    return result;
            //}

            //if (typeof(ICreationAuditedEntity).IsAssignableFrom(typeof(T)))
            //{
            //    entities = entities.Select(entity =>
            //    {
            //        var creationAuditedEntity = entity as ICreationAuditedEntity;
            //        creationAuditedEntity.CreatorUserId = KardSession.UserId;
            //        creationAuditedEntity.CreationTime = DateTime.Now;
            //        return creationAuditedEntity as T;
            //    });
            //}

            return ConnExecute(conn => conn.CreateList(entities, null, commandTimeout));
   
        }
        #endregion

        #region Retrieve
        public IEnumerable<dynamic> QueryList(string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {
            return ConnExecute(conn => conn.QueryList(sql, parameters, null, commandTimeout, commandType));
        }

        public IEnumerable<object> QueryList(Type type, string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {
 
            return ConnExecute(conn => conn.QueryList(type, sql, parameters, null, commandTimeout, commandType));
        }

        //parameters  IDictionary<string, object>,new {}
        public IEnumerable<T> QueryList<T>(string sql, object parameters = null,int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {

            return ConnExecute(conn => conn.QueryList<T>(sql, parameters, null, commandTimeout, commandType));
        }

        public IEnumerable<T> QueryList<T>(object predicate = null, IList<ISort> sort = null,  int? commandTimeout = null) where T : class
        {
            return ConnExecute(conn => conn.QueryList<T>(predicate, sort, null, commandTimeout));
        }

        public Task<IEnumerable<T>> QueryListAsync<T>(object predicate = null, IList<ISort> sort = null,  int? commandTimeout = null) where T : class
        {
            return Task.FromResult(QueryList<T>(predicate, sort, commandTimeout));
        }

        public T FirstOrDefault<T, TKey>(TKey id, int? commandTimeout = null) where T : class
        {
            return ConnExecute(conn => conn.FirstOrDefault<T, TKey>(id, null, commandTimeout));
        }

        public Task<T> FirstOrDefaultAsync<T, TKey>(TKey id, int? commandTimeout = null) where T : class
        {
            return Task.FromResult(FirstOrDefault<T, TKey>(id, commandTimeout));
        }



        public T FirstOrDefault<T>(object predicate,  int? commandTimeout = null) where T : class
        {
            return   ConnExecute(conn => conn.FirstOrDefault<T>(predicate, null, commandTimeout));
        }


        public int Count(string sql, object parameters = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {
            return ConnExecute(conn => conn.Count(sql, parameters, null, commandTimeout, commandType));
        }

        public int Count<T>(object predicate, int? commandTimeout = default(int?)) where T : class
        {
            return ConnExecute(conn => conn.Count<T>(predicate, null, commandTimeout));
        }

        //public int Count(string tableName, IDictionary<string, object> parameters = null, bool serializeParameters = false,  IDbTransaction transaction = null, int? commandTimeout = null)
        //{
        //    string sql = $"select count(1) TotalCount from {tableName}";
        //    if (serializeParameters)
        //    {
        //        sql += SerializeParameters(parameters);
        //    }

        //    if (connection != null)
        //    {
        //        return connection.ExecuteScalar<int>(sql, parameters, transaction, commandTimeout);
        //    }

        //    return ConnExecute(conn => conn.ExecuteScalar<int>(sql, parameters, null, commandTimeout));
        //}







        //public string SerializeParameters(IDictionary<string, object> parameters = null)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    if (parameters != null)
        //    {
        //        sql.Append(" where 1=1 ");
        //        foreach (string key in parameters.Keys)
        //        {
        //            sql.AppendFormat(" and {0}=@{1} ", key, key);
        //        }
        //    }

        //    return sql.ToString();
        //}
        #endregion



        #region Update
        public ResultDto Update<T>(T entity, int? commandTimeout = null) where T : class
        {
            var resultDto = new ResultDto();
            //var isLoggedIn = KardSession.UserId != null;

            //if (isLoggedIn && typeof(ILastModificationAuditedEntity).IsAssignableFrom(typeof(T)))
            //{
            //    var lastModificationAuditedEntity = entity as ILastModificationAuditedEntity;
            //    lastModificationAuditedEntity.LastModifierUserId = KardSession.UserId;
            //    lastModificationAuditedEntity.LastModificationTime = DateTime.Now;
            //    entity = lastModificationAuditedEntity as T;
            //}
         


            resultDto.Result= ConnExecute(conn => conn.Update(entity, null, commandTimeout));
            return resultDto;
        }

        public Task<ResultDto> UpdateAsync<T>(T entity, int? commandTimeout = null) where T : class
        {
            return Task.FromResult(Update(entity, commandTimeout));
        }

        public ResultDto UpdateList<T>(IEnumerable<T> entityList,int? commandTimeout = null) where T : class
        {
             return TransExecute((conn, trans) => conn.UpdateList(entityList, trans, commandTimeout));
        }
        #endregion

        #region Delete
        public ResultDto Delete<T>(T entity, int? commandTimeout = null) where T : class
        {
            //var resultDto = new ResultDto();
            //var logicDeleteEntity = entity as IDeletionAuditedEntity;
            //if (logicDeleteEntity == null)
            //{

            //    resultDto.Result= ConnExecute(conn => conn.Delete<T>(entity, transaction, commandTimeout));
            //    return resultDto;
            //}

            //logicDeleteEntity.IsDeleted = true;
            //logicDeleteEntity.DeleterUserId = KardSession.UserId;
            //logicDeleteEntity.DeletionTime = DateTime.Now;


            //resultDto.Result = ConnExecute(conn => conn.Update(logicDeleteEntity, transaction, commandTimeout));
            //return resultDto;

            var resultDto = new ResultDto();
            resultDto.Result = ConnExecute(conn => conn.Delete<T>(entity, null, commandTimeout));
            return resultDto;
        }

        public Task<ResultDto> DeleteAsync<T>(T entity,  int? commandTimeout = null) where T : class
        {
            return Task.FromResult(Delete(entity, commandTimeout));
        }

        public ResultDto DeleteList<T>(object predicate, int? commandTimeout = null) where T : class
        {
            //var resultDto = new ResultDto();
            //var isPhysicsDelete = !typeof(IDeletionAuditedEntity).IsAssignableFrom(typeof(T));

            //if (isPhysicsDelete)
            //{

            //    resultDto.Result = ConnExecute(conn => conn.Delete<T>(predicate, transaction, commandTimeout));
            //    return resultDto;
            //}

            //Func<IDbConnection, IDbTransaction, bool> updateAction = delegate (IDbConnection conn, IDbTransaction trans)
            //{
            //    var entityList = conn.GetList<T>(predicate, null, trans, commandTimeout);
            //    foreach (var entity in entityList)
            //    {
            //        var deleteAuditedEntity = entity as IDeletionAuditedEntity;
            //        deleteAuditedEntity.IsDeleted = true;
            //        deleteAuditedEntity.DeleterUserId = KardSession.UserId;
            //        deleteAuditedEntity.DeletionTime = DateTime.Now;
            //        if (!conn.Update(deleteAuditedEntity as T, trans, commandTimeout))
            //        { return false; }
            //    }
            //    return true;
            //};


            //resultDto.Result= TransExecute((conn, trans) => updateAction(conn, trans));
            //return resultDto;


            var resultDto = new ResultDto();
            resultDto.Result = ConnExecute(conn => conn.Delete<T>(predicate, null, commandTimeout));
            return resultDto;
        }


        #endregion





        #endregion
    }
}
