using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class TestRepository: Repository,ITestRepository
    {
        public TestRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public string Hello2()
        {
            return "Hello2";
        }
    }
}
