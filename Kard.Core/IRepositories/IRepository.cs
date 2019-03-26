using DapperExtensionsCore;
using Kard.Core.Dtos;
using Kard.DI;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Kard.Core.IRepositories
{

    public interface IRepository : ISingletonService
    {
        string ConnectionString { get; }

        IConfiguration Configuration { get; }

        IDbConnection DbConnection { get; }

        TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate);

        TResult TransExecute<TResult>(Func<IDbConnection, IDbTransaction, TResult> predicate, IsolationLevel? isolationLevel = null);


        //IDatabase GetDb();

        //IDbConnection GetConnection();

        //TResult ConnExecute<TResult>(Func<IDbConnection, TResult> predicate);

        //TResult TransExecute<TResult>(Func<IDbConnection, IDbTransaction, TResult> predicate, IsolationLevel? isolationLevel = null);

        //TResult DbExecute<TResult>(Func<IDatabase, TResult> predicate);


    }
}
