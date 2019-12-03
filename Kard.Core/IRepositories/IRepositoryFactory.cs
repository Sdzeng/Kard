using Kard.DI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface IRepositoryFactory:ISingletonService
    {
        IDefaultRepository Default { get;}

        T GetRepository<T>() where T : class, IRepository;
    }
}
