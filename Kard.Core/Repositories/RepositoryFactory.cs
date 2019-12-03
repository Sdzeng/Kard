using Kard.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kard.Core.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {

        private readonly IEnumerable<IRepository> _repositories;
        public RepositoryFactory(IEnumerable<IRepository> repositories)
        {
            _repositories = repositories;
        }

        public IDefaultRepository Default
        {
            get
            {
                return GetRepository<IDefaultRepository>();
            }
        }

        public T GetRepository<T>() where T : class, IRepository
        {
            return _repositories.FirstOrDefault(repository => typeof(T).IsAssignableFrom(repository.GetType())) as T;
        }
    }
}
