using Kard.Core.IRepositories;
using Kard.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public abstract class Repository: IRepository
    {
        private static string _connectionString;

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

        protected IDbConnection GetConnection()
        {
                return new MySqlConnection(ConnectionString);
        }
    }
}
