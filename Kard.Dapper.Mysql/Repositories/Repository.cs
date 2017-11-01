using DapperExtensions.Core20;
using DapperExtensions.Core20.Sql;
using Kard.Core.IRepositories;
using Kard.Core.Mappers;
using Kard.Domain.Entities.Auditing;
using Kard.Extensions;
using Microsoft.Extensions.Configuration;
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
    public abstract class Repository: IRepository
    {
        private static IDapperExtensionsConfiguration _config;
        private static string _connectionString;

        static Repository()
        {

            //DapperExtensions.Core20.DapperExtensions.DefaultMapper = typeof(CustomPluralizedAutoClassMapper<>);
            //DapperExtensions.Core20.DapperExtensions.SqlDialect = new MySqlDialect();
            //DapperExtensions.DapperExtensions.Configure(
            //    typeof(CustomPluralizedAutoClassMapper<>),
            //    new List<Assembly>(),
            //    new MySqlDialect());
            _config = new DapperExtensionsConfiguration(typeof(CustomPluralizedAutoClassMapper<>), new List<Assembly>(), new MySqlDialect());

            DapperExtensions.Core20.DapperExtensions.Configure(_config);
            DapperExtensions.Core20.DapperAsyncExtensions.Configure(_config);
        }

        public Repository(IConfiguration configuration)
        {
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

        protected IDatabase GetDb()
        {
            var connection = GetConnection();
            var sqlGenerator = new SqlGeneratorImpl(_config);
            IDatabase _db = new Database(connection, sqlGenerator);
            return _db;
        }

        protected IDbConnection GetConnection()
        {
                return new MySqlConnection(ConnectionString);
        }


        protected TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate)
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                var result = predicate.Invoke(connection);
                connection.Close();
                return result;
            }
        }

        protected TResult DbExecute<TResult>(Func<IDatabase, TResult> predicate)
        {
            using (IDatabase db = GetDb())
            {
                var result = predicate.Invoke(db);
                return result;
            }
        }

        #region CRUD

        #region Create
        public virtual bool Create<T>(T entity) where T : class, ICreationAuditedEntity
        {
            //entity.CreatorUserId = WmsSession.UserId;
            entity.CreationTime = DateTime.Now;
            var result = ConnExecute(connection => connection.Insert(entity));
            return (result == 1);
        }

        public virtual Task<bool> CreateAsync<T>(T entity) where T : class, ICreationAuditedEntity
        {
            return Task.FromResult(Create(entity));
        }
        #endregion

        #region Retrieve
        public T FirstOrDefault<TKey, T>(TKey id) where T : class
        {
            return ConnExecute(connection => connection.Get<T>(id));
        }

        public Task<T> FirstOrDefaultAsync<TKey, T>(TKey id) where T : class
        {
            return Task.FromResult(FirstOrDefault<TKey, T>(id));
        }

        public T UniquenessOrDefault<T>(object predicate) where T : class
        {
            var entityList = GetList<T>(predicate);
            if (entityList?.Count() != 1)
            {
                return null;
            }

            return entityList.First();
        }

        public IEnumerable<T> GetList<T>() where T : class
        {
            return ConnExecute(connection => connection.GetList<T>());
        }

        public Task<IEnumerable<T>> GetListAsync<T>() where T : class
        {
            return Task.FromResult(GetList<T>());
        }

        public IEnumerable<T> GetList<T>(object predicate) where T : class
        {
            return ConnExecute(connection => connection.GetList<T>(predicate));
        }

        public Task<IEnumerable<T>> GetListAsync<T>(object predicate) where T : class
        {
            return Task.FromResult(GetList<T>(predicate));
        }

        public IEnumerable<T> GetPage<T>(object predicate, int page, int resultsPerPage) where T : class
        {
            IList<ISort> sortList = new List<ISort> { new Sort { Ascending = false, PropertyName = "Id" } };
            return ConnExecute(connection => connection.GetPage<T>(predicate, sortList, page, resultsPerPage));
        }
        #endregion

        #region Update
        public virtual bool Update<T>(T entity) where T : class, ILastModificationAuditedEntity
        {
            //var isLoggedIn = WmsSession.UserId != null;

            //if (isLoggedIn)
            //{
            //    entity.LastModifierUserId = WmsSession.UserId;
            //    entity.LastModificationTime = DateTime.Now;
            //}

            var result = ConnExecute(connection => connection.Update(entity));
            return result;
        }

        public virtual Task<bool> UpdateAsync<T>(T entity) where T : class, ILastModificationAuditedEntity
        {
            return Task.FromResult(Update(entity));
        }
        #endregion

        #region Delete
        public virtual bool Delete<T>(T entity) where T : class
        {
            var result = false;
            var logicDeleteEntity = entity as IDeletionAuditedEntity;
            if (logicDeleteEntity == null)
            {
                result = ConnExecute(connection => connection.Delete(entity));
            }
            else
            {
                logicDeleteEntity.IsDeleted = true;
                //logicDeleteEntity.DeleterUserId = WmsSession.UserId;
                logicDeleteEntity.DeletionTime = DateTime.Now;
                result = ConnExecute(connection => connection.Update(logicDeleteEntity));
            }

            return result;
        }

        public virtual Task<bool> DeleteAsync<T>(T entity) where T : class
        {
            return Task.FromResult(Delete(entity));
        }
        #endregion

        #endregion

    }
}
