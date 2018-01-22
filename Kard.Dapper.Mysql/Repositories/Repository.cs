using Dapper;
using Kard.Core.Dtos;
using Kard.Core.IRepositories;
 
using Kard.Domain.Entities.Auditing;
using Kard.Extensions;
using Kard.Runtime.Session;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public abstract class Repository : IRepository
    {
        //private static IDapperExtensionsConfiguration _dapperConfig;
        //private static IDapperImplementor _dapperInstance;
        //private static IDapperAsyncImplementor _dapperAsyncInstance;
        private static string _connectionString;

        //static Repository()
        //{
        //    //1
        //    //DapperExtensions.Core20.DapperExtensions.DefaultMapper = typeof(CustomPluralizedAutoClassMapper<>);
        //    //DapperExtensions.Core20.DapperExtensions.SqlDialect = new MySqlDialect();
        //    //DapperExtensions.DapperExtensions.Configure(
        //    //    typeof(CustomPluralizedAutoClassMapper<>),
        //    //    new List<Assembly>(),
        //    //    new MySqlDialect());

        //    _dapperConfig = new DapperExtensionsConfiguration(typeof(CustomPluralizedAutoClassMapper<>), new List<Assembly>(), new MySqlDialect());
        //    //使用connection
        //    DapperExtensions.Core20.DapperExtensions.Configure(_dapperConfig);
        //    DapperExtensions.Core20.DapperAsyncExtensions.Configure(_dapperConfig);
        //    //使用instance
        //    _dapperInstance = DapperExtensions.Core20.DapperExtensions.InstanceFactory.Invoke(_dapperConfig);
        //    _dapperAsyncInstance = DapperExtensions.Core20.DapperAsyncExtensions.InstanceFactory.Invoke(_dapperConfig);
        //}




        public Repository(IKardSession session, IConfiguration configuration)
        {
            Session = session;
            Configuration = configuration;
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



        public IKardSession Session { get; set; }



        //public IDatabase GetDb()
        //{
        //    var connection = GetConnection();
        //    var sqlGenerator = new SqlGeneratorImpl(_dapperConfig);
        //    IDatabase _db = new Database(connection, sqlGenerator);
        //    return _db;
        //}

        public IDbConnection GetConnection()
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

        public TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate)
        {
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    var result = predicate.Invoke(connection);
                    connection.Close();

                    return result;
                }
            }
            catch (Exception e)
            {
                return default(TResult);
            }
        }

        public TResult TransExecute<TResult>(Func<IDbConnection, IDbTransaction, TResult> predicate)
        {
            TResult result;
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            result = predicate.Invoke(connection, transaction);
                            if (typeof(TResult) == typeof(bool) && Convert.ToBoolean(result) == false)
                            {
                                throw new Exception("TransExecute的Func执行结果为false");
                            }

                            if (typeof(TResult) == typeof(ResultDto) && (result as ResultDto).Result == false)
                            {
                                throw new Exception("TransExecute的Func执行结果为false");
                            }

                            transaction.Commit();
                        }
                        catch (Exception e1)
                        {
                            transaction.Rollback();
                            throw;
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
                result = default(TResult);
            }

            return result;
        }

        //public TResult DbExecute<TResult>(Func<IDatabase, TResult> predicate)
        //{
        //    using (IDatabase db = GetDb())
        //    {
        //        var result = predicate.Invoke(db);
        //        return result;
        //    }
        //}

        #endregion

        #region CRUD

        #region Create
        //public ResultDto<TKey> CreateAndGetId<T, TKey>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    var result = new ResultDto<TKey>();

        //    if (typeof(ICreationAuditedEntity).IsAssignableFrom(typeof(T)))
        //    {
        //        var creationAuditedEntity = entity as ICreationAuditedEntity;
        //        creationAuditedEntity.CreatorUserId = Session.UserId;
        //        creationAuditedEntity.CreationTime = DateTime.Now;
        //        entity = creationAuditedEntity as T;
        //    }

        //    TKey id = default(TKey);

        //    if (connection != null)
        //    {
        //        id = connection.Insert<T>(entity, transaction, commandTimeout);
        //    }
        //    else
        //    {
        //        id = ConnExecute(conn => conn.Insert(entity, null, commandTimeout));
        //    }

        //    result.Result = (!id.Equals(default(TKey)));
        //    if (!result.Result)
        //    {
        //        result.Message = "新增失败";
        //    }
        //    else
        //    {
        //        result.Data = id;
        //    }
        //    return result;
        //}

 

        //public bool Create<T>(IEnumerable<T> entities, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (entities == null || (!entities.Any()))
        //    { return false; }

        //    if (typeof(ICreationAuditedEntity).IsAssignableFrom(typeof(T)))
        //    {
        //        entities = entities.Select(entity =>
        //        {
        //            var creationAuditedEntity = entity as ICreationAuditedEntity;
        //            creationAuditedEntity.CreatorUserId = Session.UserId;
        //            creationAuditedEntity.CreationTime = DateTime.Now;
        //            return creationAuditedEntity as T;
        //        });
        //    }

        //    try
        //    {
        //        if (connection != null)
        //        {
        //            connection.Insert(entities, transaction, commandTimeout);
        //        }
        //        else
        //        {
        //            ConnExecute(conn => { conn.Insert(entities, null, commandTimeout); return true; });
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
   
        
        #endregion

        #region Retrieve



        public IEnumerable<T> Query<T>(string sql,object parameters = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = default(CommandType?))
        {

            if (connection != null)
            {
                return connection.Query<T>(sql, parameters, transaction, false, commandTimeout, commandType);
            }

            return ConnExecute(conn => conn.Query<T>(sql, parameters, null, false, commandTimeout, commandType));
        }

        //public T FirstOrDefault<TKey, T>(TKey id, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (connection != null)
        //    {
        //        return connection.Get<T>(id, transaction, commandTimeout);
        //    }

        //    return ConnExecute(conn => conn.Get<T>(id, null, commandTimeout));
        //}

 
 

        //public T UniquenessOrDefault<T>(object predicate, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    var entityList = GetList<T>(predicate, null, connection, transaction, commandTimeout);
        //    if (entityList?.Count() != 1)
        //    {
        //        return default(T);
        //    }

        //    return entityList.First();
        //}


        public long GetCount(string tableName, IDictionary<string, object> parameters = null, bool serializeParameters = false, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            string sql = $"select count(1) TotalCount from {tableName}";
            if (serializeParameters)
            {
                sql += SerializeParameters(parameters);
            }

            if (connection != null)
            {
                return connection.ExecuteScalar<int>(sql, parameters, transaction, commandTimeout);
            }

            return ConnExecute(conn => conn.ExecuteScalar<int>(sql, parameters, null, commandTimeout));
        }


        //public IEnumerable<T> GetList<T>(object predicate = null, IList<ISort> sort = null, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (connection != null)
        //    {
        //        return connection.GetList<T>(predicate, sort, transaction, commandTimeout).ToList();
        //    }

        //    return ConnExecute(conn => conn.GetList<T>(predicate, sort, null, commandTimeout).ToList());
        //}

 

  

        protected string SerializeParameters(IDictionary<string, object> parameters = null)
        {
            StringBuilder sql = new StringBuilder();
            if (parameters != null)
            {
                sql.Append(" where 1=1 ");
                foreach (string key in parameters.Keys)
                {
                    sql.AppendFormat(" and {0}=@{1} ", key, key);
                }
            }

            return sql.ToString();
        }
        #endregion

        #region Update
        //public bool Update<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    var isLoggedIn = Session.UserId != null;

        //    if (isLoggedIn && typeof(ILastModificationAuditedEntity).IsAssignableFrom(typeof(T)))
        //    {
        //        var lastModificationAuditedEntity = entity as ILastModificationAuditedEntity;
        //        lastModificationAuditedEntity.LastModifierUserId = Session.UserId;
        //        lastModificationAuditedEntity.LastModificationTime = DateTime.Now;
        //        entity = lastModificationAuditedEntity as T;
        //    }

        //    if (connection != null)
        //    {
        //        return connection.Update(entity, transaction, commandTimeout);
        //    }

        //    return ConnExecute(conn => conn.Update(entity, null, commandTimeout));
        //}

  
        #endregion

        #region Delete
        //public bool Delete<T>(T entity, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    var logicDeleteEntity = entity as IDeletionAuditedEntity;
        //    if (logicDeleteEntity == null)
        //    {
        //        if (connection != null)
        //        {
        //            return connection.Delete<T>(entity, transaction, commandTimeout);
        //        }

        //        return ConnExecute(conn => conn.Delete<T>(entity, null, commandTimeout));
        //    }

        //    logicDeleteEntity.IsDeleted = true;
        //    logicDeleteEntity.DeleterUserId = Session.UserId;
        //    logicDeleteEntity.DeletionTime = DateTime.Now;
        //    if (connection != null)
        //    {
        //        return connection.Update(logicDeleteEntity, transaction, commandTimeout);
        //    }

        //    return ConnExecute(conn => connection.Update(logicDeleteEntity, null, commandTimeout));
        //}

 

        //public bool Delete<T>(object predicate, IDbConnection connection = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    var isPhysicsDelete = !typeof(IDeletionAuditedEntity).IsAssignableFrom(typeof(T));

        //    if (isPhysicsDelete)
        //    {
        //        if (connection != null)
        //        {
        //            return connection.Delete<T>(predicate, transaction, commandTimeout);
        //        }
        //        return ConnExecute(conn => conn.Delete<T>(predicate, null, commandTimeout));
        //    }

        //    Func<IDbConnection, IDbTransaction, bool> updateAction = delegate (IDbConnection conn, IDbTransaction trans)
        //    {
        //        var entityList = GetList<T>(predicate, null, conn, trans, commandTimeout);
        //        foreach (var entity in entityList)
        //        {
        //            var deleteAuditedEntity = entity as IDeletionAuditedEntity;
        //            deleteAuditedEntity.IsDeleted = true;
        //            deleteAuditedEntity.DeleterUserId = Session.UserId;
        //            deleteAuditedEntity.DeletionTime = DateTime.Now;
        //            if (!conn.Update(deleteAuditedEntity, trans, commandTimeout))
        //            { return false; }
        //        }
        //        return true;
        //    };


        //    if (connection != null)
        //    {
        //        return updateAction(connection, transaction);
        //    }

        //    return TransExecute((conn, trans) => updateAction(conn, trans));
        //}


        #endregion




        #endregion
    }
}
