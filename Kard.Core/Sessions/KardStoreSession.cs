using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.Sessions
{
    public class KardStoreSession : KardSession
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IRepositoryFactory _repositoryFactory;

        public KardStoreSession(IMemoryCache memoryCache, IRepositoryFactory repositoryFactory, IPrincipalAccessor principalAccessor) : base(principalAccessor)
        {
            _memoryCache = memoryCache;
            _repositoryFactory = repositoryFactory;
        }


        public override SessionData Data
        {
            get
            {
                string cacheKey = $"UserData[{this.UserId}]";
                return _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
                 {
                     cacheEntry.SetAbsoluteExpiration(DateTime.Now.Date.AddDays(60));

                     var kuserEntity = _repositoryFactory.Default.FirstOrDefault<KuserEntity>(this.UserId);
                     return new SessionData
                     {
                         NickName = kuserEntity.NickName,
                         Phone = kuserEntity.Phone
                     };
                 });
            }
        }





        public override void RefreshData()
        {
            string cacheKey = $"UserData[{this.UserId}]";
            _memoryCache.Remove(cacheKey);
        }
    }
}
