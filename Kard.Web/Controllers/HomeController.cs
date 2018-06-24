using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Kard.Web.Controllers
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("api/home")]
    public class HomeController : BaseController
    {
        private readonly IDefaultRepository _defaultRepository;
        public HomeController(ILogger<HomeController> logger,
            IMemoryCache memoryCache,
            IDefaultRepository defaultRepository,
            IKardSession kardSession) : base(logger, memoryCache, kardSession)
        {
            _defaultRepository = defaultRepository;
        }


        /// <summary>
        /// 获取封面
        /// </summary>
        /// <returns></returns>
        [HttpGet("cover")]
        public ResultDto<CoverEntity> GetCover()
        {
            var today = DateTime.Now.Date;
            string cacheKey = $"homeCover[{today.ToString("yyyyMMdd")}]";
            CoverEntity coverEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
                return _defaultRepository.GetDateCover(today);
            });
            return new ResultDto<CoverEntity>() { Result = true, Data = coverEntity };
        }


        /// <summary>
        /// 获取单品图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("hostpictures")]
        public ResultDto<IEnumerable<TopMediaDto>> GetHostPictures()
        {
            var resultDto = new ResultDto<IEnumerable<TopMediaDto>>();


            resultDto.Result = true;
            resultDto.Data = _defaultRepository.GetHomeMediaPictureList(12, "热门单品");

            //var aWeekAgo = DateTime.Now.Date.AddYears(-7);
            return resultDto;
        }


        /// <summary>
        /// 获取单品图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("categorypictures")]
        public ResultDto GetCategoryPictures()
        {
            var resultDto = new ResultDto();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                resultDto.Result = true;
                resultDto.Data = new
                {
                    CosmeticsList = _defaultRepository.GetHomeMediaPictureList(12, "妆品"),
                    FashionSenseList = _defaultRepository.GetHomeMediaPictureList(12, "潮拍"),
                    OriginalityList = _defaultRepository.GetHomeMediaPictureList(12, "创意")
                    //ExcerptList = _defaultRepository.GetHomeMediaPicture(12, "摘录")
                };
                //var aWeekAgo = DateTime.Now.Date.AddYears(-7);
                return resultDto;
            }
            finally
            {
                sw.Stop();
                _logger.LogDebug($"GetPicture耗时：{sw.ElapsedMilliseconds}");
            }
        }


        /// <summary>
        /// 获取单品图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("essay")]
        public ResultDto GetEssay(long id)
        {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            var essayEntity = _defaultRepository.GetEssay(id);
            //sw.Stop();
            //_logger.LogDebug($"GetEssay耗时：{sw.ElapsedMilliseconds}");
            //sw.Restart();
            //essayEntity= _defaultRepository.GetEssay2(id);
            //sw.Stop();
            //_logger.LogDebug($"GetEssay2耗时：{sw.ElapsedMilliseconds}");
            var resultDto = new ResultDto();
            resultDto.Result = essayEntity!=null;
            resultDto.Data = essayEntity;
            return resultDto;
        }

    }
}