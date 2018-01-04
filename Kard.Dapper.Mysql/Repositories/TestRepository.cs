using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class TestRepository: Repository,ITestRepository
    {
        public TestRepository(IKardSession session, IConfiguration configuration) : base(session, configuration) { 

        }

        public string Hello2()
        {
            return "Hello2";
        }
    }
}
