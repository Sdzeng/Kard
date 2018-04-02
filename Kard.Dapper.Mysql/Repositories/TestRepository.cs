using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class TestRepository: Repository,ITestRepository
    {
        public TestRepository(IKardSession session, IConfiguration configuration, ILogger<DefaultRepository> logger) : base(session, configuration, logger) { 

        }

        public string Hello2()
        {
            return "Hello2";
        }
    }
}
