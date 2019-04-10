using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kard.Web.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/count")]
    public class CountController : BaseController
    {
        private readonly IDefaultRepository _defaultRepository;

        public CountController(
         ILogger<CountController> logger,
         IMemoryCache memoryCache,
         IKardSession kardSession,
         IDefaultRepository defaultRepository)
         : base(logger, memoryCache, kardSession)
        {
            _defaultRepository = defaultRepository;
        }
       /// <summary>
       /// 统计阅读
       /// </summary>
       /// <param name="id"></param>
        [HttpGet("essay")]
        public void Essay(int id) {

            _logger.LogDebug("开始测试异步非阻塞");

            Task.Run(() => {
                _logger.LogDebug("执行统计");
                var result =_defaultRepository.UpdateBrowseNum(id);
                _logger.LogDebug($"完成统计{result.ToString()}");
            });
            _logger.LogDebug("完成测试异步非阻塞");
        }
 
    }
}