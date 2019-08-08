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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kard.Web.Controllers
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("home")]
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
        public async Task<ResultDto<CoverDto>> GetCover()
        {
            var today = DateTime.Now.Date;
            //string cacheKey = $"homeCover[{today.ToString("yyyyMMdd")}]";
            //CoverDto coverDto = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            //{

            //    cacheEntry.SetAbsoluteExpiration(today.AddDays(1));
            //    return _defaultRepository.Cover.GetDateCover(today);
            //});
            //coverDto.EssayContent = Utils.ContentRegex.Replace(coverDto.EssayContent, "");
            //return new ResultDto<CoverDto>() { Result = true, Data = coverDto };

            var coverDto = _defaultRepository.Cover.GetDateCover(today);
            if (coverDto.EssayContent.Contains("。"))
            {
                coverDto.EssayContent = coverDto.EssayContent.Split("。")[0] + "。";
            }
       
            return await Task.FromResult(new ResultDto<CoverDto>() { Result = true, Data = coverDto });

        }


        /// <summary>
        /// 获取单品列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("essays")]
        public async Task<ResultDto> GetEssays(string keyword, string orderBy,int pageIndex = 1, int pageSize = 20)
        {
            var essayList = _defaultRepository.Essay.GetHomeMediaPictureList(keyword, pageIndex, pageSize + 1, orderBy) ?? new List<TopMediaDto>();
             
            var hasNextPage = essayList.Count() > pageSize;
            var resultDto = new ResultDto();
            resultDto.Result = true;
            resultDto.Data = new
            {
                hasNextPage,
                essayList = hasNextPage ? essayList.SkipLast(1) : essayList
            };

            //var aWeekAgo = DateTime.Now.Date.AddYears(-7);
            return await Task.FromResult(resultDto);
        }

        ///// <summary>
        ///// 获取单品图片
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("hostpictures")]
        //public ResultDto<IEnumerable<TopMediaDto>> GetHostPictures()
        //{
        //    var resultDto = new ResultDto<IEnumerable<TopMediaDto>>();


        //    resultDto.Result = true;
        //    resultDto.Data = _defaultRepository.GetHomeMediaPictureList(12, "热门单品");

        //    //var aWeekAgo = DateTime.Now.Date.AddYears(-7);
        //    return resultDto;
        //}


        ///// <summary>
        ///// 获取单品图片
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("categorypictures")]
        //public ResultDto GetCategoryPictures()
        //{
        //    var resultDto = new ResultDto();
        //    var sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    try
        //    {
        //        resultDto.Result = true;
        //        resultDto.Data = new
        //        {
        //            CosmeticsList = _defaultRepository.GetHomeMediaPictureList(12, "物件"),
        //            OriginalityList = _defaultRepository.GetHomeMediaPictureList(12, "设计"),
        //            FashionSenseList = _defaultRepository.GetHomeMediaPictureList(12, "潮拍")

        //            //ExcerptList = _defaultRepository.GetHomeMediaPicture(12, "摘录")
        //        };
        //        //var aWeekAgo = DateTime.Now.Date.AddYears(-7);
        //        return resultDto;
        //    }
        //    finally
        //    {
        //        sw.Stop();
        //        _logger.LogDebug($"GetPicture耗时：{sw.ElapsedMilliseconds}");
        //    }
        //}






    }
}