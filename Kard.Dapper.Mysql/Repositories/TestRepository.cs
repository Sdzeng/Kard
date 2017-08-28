using Kard.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class TestRepository: ITestRepository
    {
        public string Hello2()
        {
            return "Hello2";
        }
    }
}
