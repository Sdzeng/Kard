using Kard.Core.AppServices.Baiduspider;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Extensions;
using Kard.Json;
using Kard.Runtime.Security.Authentication.WeChat;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Web.Controllers
{
    [Produces("application/json")]
    [Route("essay")]
    public class EssayController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IDefaultRepository _defaultRepository;
        private readonly IConfiguration _configuration;
        private readonly IBaiduspiderAppService _baiduspiderAppService;

        public EssayController(IHostingEnvironment env,
            IServiceProvider serviceProvider,
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IRepositoryFactory repositoryFactory,
            ILogger<EssayController> logger,
            IMemoryCache memoryCache,
            IKardSession kardSession,
            IConfiguration configuration,
            IBaiduspiderAppService baiduspiderAppService) : base(logger, memoryCache, kardSession)
        {
            _env = env;
            _serviceProvider = serviceProvider;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _repositoryFactory = repositoryFactory;
            _defaultRepository = repositoryFactory.GetRepository<IDefaultRepository>();
            _configuration = configuration;
            _baiduspiderAppService = baiduspiderAppService;
        }

        #region essay





        /// <summary>
        /// 获取单品信息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ResultDto GetInfo(long id)
        {
            //单品信息
            var essayEntity = _repositoryFactory.GetRepository<IEssayRepository>().GetEssayDto(id, _kardSession.UserId);

            var resultDto = new ResultDto();
            resultDto.Result = essayEntity != null;
            resultDto.Data = essayEntity;
            //评论 喜欢

            //增加阅读数
            Task.Run(() =>
            {
                var result = _repositoryFactory.GetRepository<IEssayRepository>().UpdateBrowseNum(id);
                if (!result)
                {
                    _logger.LogError($"统计：单品{id}增加阅读数失败");
                }
            });

            return resultDto;
        }





        /// <summary>
        /// 获取单品信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("updateinfo")]
        public async Task<ResultDto> GeUpdateInfo(long id)
        {
            //单品信息
            var essayEntity = await _defaultRepository.FirstOrDefaultAsync<EssayEntity>(id);
            var essayContentEntity = _defaultRepository.FirstOrDefaultByPredicate<EssayContentEntity>(new { EssayId = id });
            var tagList = _defaultRepository.QueryByPredicate<TagEntity>(new { EssayId = id });
            var resultDto = new ResultDto();
            resultDto.Result = essayEntity != null;
            resultDto.Data = new { essay = essayEntity, essayContent = essayContentEntity, tagList };

            return await Task.FromResult(resultDto);
        }

        ///// <summary>
        ///// froala上传文件
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("froalaupload")]
        //[AllowAnonymous]
        ////[Consumes("multipart/form-data")]
        ////[RequestSizeLimit(100_000_000)]
        //public object FroalaUpload(IFormFile mediaFlie)
        //{
        //    var result = new ResultDto();
        //    if (mediaFlie == null && Request.Form.Files.Any())
        //    {
        //        mediaFlie = Request.Form.Files[0];
        //    }
        //    if (mediaFlie == null)
        //    {
        //        result.Result = true;
        //        result.Message = "未选择文件";
        //        return result;
        //    }

        //    var now = DateTime.Now;
        //    string webRootPath = _env.WebRootPath;

        //    string newFolder = Path.Combine("user", _kardSession.UserId.ToString(), "media", now.ToString("yyyyMMdd"));
        //    string newPath = Path.Combine(webRootPath, newFolder);
        //    if (!Directory.Exists(newPath))
        //    {
        //        Directory.CreateDirectory(newPath);
        //    }

        //    if (mediaFlie.Length > 0)
        //    {
        //        string fileName = now.ToString("ddHHmmssffff");
        //        string fileExtension = Path.GetExtension(mediaFlie.FileName.Trim('"')).ToLower(); // Path.GetExtension(ContentDispositionHeaderValue.Parse(mediaFlie.ContentDisposition).FileName.Trim('"'));
        //        string fullPath = Path.Combine(newPath, fileName + fileExtension);
        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            mediaFlie.CopyTo(stream);
        //        }

        //        return new { Link = Path.Combine(_defaultRepository.Configuration.GetValue<string>("AppSetting:ApiDomain"), newFolder, fileName+fileExtension).Replace("\\", "/") };
        //    }


        //    result.Result = false;
        //    result.Message = "上传失败";
        //    return result;
        //}

        ///// <summary>
        ///// 上传文件
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("uploadmedia")]
        ////[Consumes("multipart/form-data")]
        ////[RequestSizeLimit(100_000_000)]
        //public ResultDto UploadMedia(IFormFile mediaFlie)
        //{
        //    var result = new ResultDto();
        //    if (mediaFlie == null && Request.Form.Files.Any())
        //    {
        //        mediaFlie = Request.Form.Files[0];
        //    }
        //    if (mediaFlie == null)
        //    {
        //        result.Result = true;
        //        result.Message = "未选择文件";
        //        return result;
        //    }

        //    var now = DateTime.Now;
        //    string webRootPath = _env.WebRootPath;

        //    string newFolder = Path.Combine("user", _kardSession.UserId.ToString(), "media", now.ToString("yyyyMMdd"));
        //    string newPath = Path.Combine(webRootPath, newFolder);
        //    if (!Directory.Exists(newPath))
        //    {
        //        Directory.CreateDirectory(newPath);
        //    }

        //    if (mediaFlie.Length > 0)
        //    {
        //        string fileName = now.ToString("ddHHmmssffff");
        //        string fileExtension = Path.GetExtension(mediaFlie.FileName.Trim('"')).ToLower(); // Path.GetExtension(ContentDispositionHeaderValue.Parse(mediaFlie.ContentDisposition).FileName.Trim('"'));
        //        string fullPath = Path.Combine(newPath, fileName + fileExtension);
        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            mediaFlie.CopyTo(stream);
        //        }
        //        result.Result = true;
        //        result.Data = new { FileUrl = Path.Combine(newFolder, fileName).Replace("\\", "/"), FileExtension = fileExtension.Replace(".", "") };
        //        return result;
        //    }

        //    result.Result = false;
        //    result.Message = "上传失败";
        //    return result;
        //}



        /// 添加纪录
        /// </summary>
        /// <param name="essayEntity"></param>
        /// <param name="tagList"></param>
        [HttpPost("add")]
        public async Task<ResultDto<long>> Add(EssayEntity essayEntity, EssayContentEntity essayContentEntity, IEnumerable<TagEntity> tagList)
        {
            var userId = _kardSession.UserId.Value;

            essayEntity.SubContent = Utils.ContentRegex.Replace(essayContentEntity.Content, "");
            if (essayEntity.SubContent.Length > 100)
            {
                essayEntity.SubContent = essayEntity.SubContent.Remove(100) + "...";
            }

            essayEntity.IsPublish = essayEntity.IsPublish;
            essayEntity.Location = Utils.GetCity(HttpContext, _memoryCache);
            essayEntity.Score = essayEntity.Score > 0 ? essayEntity.Score : 6m;
            essayEntity.ScoreHeadCount = 1;
            essayEntity.AuditCreation(userId);
            tagList.AuditCreation(userId);
            var resultDto = _repositoryFactory.GetRepository<IEssayRepository>().AddEssay(essayEntity, essayContentEntity, tagList);

            if (resultDto.Result)
            {
                var createHtmlResult = await CreateHtml(resultDto.Data);
                essayEntity.PageUrl = createHtmlResult.Data;
                await _defaultRepository.UpdateAsync(essayEntity);

                _baiduspiderAppService.Baiduspider(essayEntity.PageUrl);
                //string cacheKey = $"homeCover[{DateTime.Now.ToString("yyyyMMdd")}]";
                //_memoryCache.Remove(cacheKey);
            }

                
            return await Task.FromResult(resultDto);
        }


        private async Task<ResultDto<string>> CreateHtml(long id, string pageUrl = null)
        {

            //单品信息
            var essayEntity = _repositoryFactory.GetRepository<IEssayRepository>().GetHtmlEssayDto(id);
            essayEntity.Meta = essayEntity.SubContent;
            if (essayEntity.Meta.Contains("。"))
            {
                essayEntity.Meta = essayEntity.Meta.Split("。")[0];
            }
            else if (essayEntity.Meta.Contains("."))
            {
                essayEntity.Meta = essayEntity.Meta.Split(".")[0];
            }
            else if (essayEntity.Meta.Length > 15)
            {
                essayEntity.Meta = essayEntity.Meta.Remove(15);
            }

            var fileName=string.Empty;
            if (string.IsNullOrEmpty(pageUrl))
            {
                fileName = $"{DateTime.Now.ToString("MMddHHmmssffff")}.html";
            }
            else
            {
                var oldFile = Path.Combine(_env.WebRootPath, pageUrl);
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }

                var oldMFile = Path.Combine(_env.WebRootPath, pageUrl.Replace("essay", Path.Combine("essay", "m")));
                if (System.IO.File.Exists(oldMFile))
                {
                    System.IO.File.Delete(oldMFile);
                }

                fileName = Path.GetFileName(pageUrl);
            }

            var folderPath = "essay";
            var mfolderPath = Path.Combine("essay", "m");
            var page = WriteViewToFileAsync("EssayDetail", essayEntity, folderPath, fileName);
            await WriteViewToFileAsync("MEssayDetail", essayEntity, mfolderPath, fileName);

            return await page;
        }

        private async Task<ResultDto<string>> WriteViewToFileAsync(string viewName, object model, string folderPath, string fileName)
        {
            var resultDto = new ResultDto<string> { Result = false };
            try
            {

                var html = await RenderToStringAsync(viewName, model);
                if (string.IsNullOrWhiteSpace(html))
                    return resultDto;





                string absolutePath = Path.Combine(_env.WebRootPath, folderPath);


                string fullPath = Path.Combine(absolutePath, fileName);
                if (!Directory.Exists(absolutePath))
                {
                    Directory.CreateDirectory(absolutePath);
                }

                System.IO.File.WriteAllText(fullPath, html, Encoding.UTF8);
                resultDto.Result = true;
                resultDto.Data = Path.Combine(folderPath, fileName).Replace("\\", "/");



            }
            catch (Exception ex)
            {
                resultDto.Message = $"生成html静态文件失败:{ex.Message}";
            }

            return await Task.FromResult(resultDto);
        }



        /// <summary>
        /// 渲染视图
        /// </summary>
        private async Task<string> RenderToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            using (var stringWriter = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(actionContext, viewName, true);
                if (viewResult.View == null)
                    throw new ArgumentNullException($"未找到视图： {viewName}");
                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };
                var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), stringWriter, new HtmlHelperOptions());
                await viewResult.View.RenderAsync(viewContext);
                return stringWriter.ToString();
            }
        }




        /// <summary>
        /// 修改纪录
        /// </summary>
        /// <param name="essayEntity"></param>
        /// <param name="essayContentEntity"></param>
        /// <param name="tagList"></param>
        [HttpPost("update")]
        public async Task<ResultDto> Update(EssayEntity essayEntity, EssayContentEntity essayContentEntity, IEnumerable<TagEntity> tagList)
        {
            var resultDto = new ResultDto();
            var userId = _kardSession.UserId.Value;

            if (essayEntity.Id <= 0)
            {
                resultDto.Result = false;
                resultDto.Message = "修改失败，Id为空";
                return resultDto;
            }

            var entity = _defaultRepository.FirstOrDefault<EssayEntity>(essayEntity.Id);
            if (entity == null)
            {
                resultDto.Result = false;
                resultDto.Message = "修改失败，未找到文章";
                return resultDto;
            }

            if (entity.CreatorUserId != userId)
            {
                resultDto.Result = false;
                resultDto.Message = "修改失败，这不是您的文章";
                return resultDto;
            }

            entity.Title = essayEntity.Title;
            entity.CoverMediaType = essayEntity.CoverMediaType;
            entity.CoverPath = essayEntity.CoverPath;
            entity.CoverExtension = essayEntity.CoverExtension;
            entity.Category = essayEntity.Category;
            entity.IsPublish = essayEntity.IsPublish;
            essayEntity.Score = essayEntity.Score > 0 ? essayEntity.Score : 6m;

            entity.SubContent = Utils.ContentRegex.Replace(essayContentEntity.Content, "");
            if (entity.SubContent.Length > 100)
            {
                entity.SubContent = entity.SubContent.Remove(100) + "...";
            };


            essayEntity.Location = Utils.GetCity(HttpContext, _memoryCache);

            entity.AuditLastModification(userId);
            tagList.AuditCreation(userId);

            var result = _repositoryFactory.GetRepository<IEssayRepository>().UpdateEssay(entity, essayContentEntity, tagList);

            if (result)
            {
                var createHtmlResult = await CreateHtml(entity.Id, entity.PageUrl);

                _baiduspiderAppService.Baiduspider(entity.PageUrl);

                result = createHtmlResult.Result;
            }
           


            if (result)
            {
                resultDto.Result = true;
                resultDto.Message = "修改成功";
                string cacheKey = $"homeCover[{DateTime.Now.ToString("yyyyMMdd")}]";
                _memoryCache.Remove(cacheKey);
            }
            else
            {
                resultDto.Result = false;
                resultDto.Message = "修改失败";
            }
            return await Task.FromResult(resultDto);
        }


        /// 删除纪录
        /// </summary>
        /// <param name="essayEntity"></param>
        /// <param name="essayContentEntity"></param>
        /// <param name="tagList"></param>
        [HttpPost("delete")]
        public async Task<ResultDto> Delete(long id)
        {
            var resultDto = new ResultDto();
            var userId = _kardSession.UserId.Value;

            if (id <= 0)
            {
                resultDto.Result = false;
                resultDto.Message = "删除失败，Id为空";
                return resultDto;
            }

            var entity = _defaultRepository.FirstOrDefault<EssayEntity>(id);
            if (entity == null)
            {
                resultDto.Result = false;
                resultDto.Message = "删除失败，未找到文章";
                return resultDto;
            }

            if (entity.CreatorUserId != userId)
            {
                resultDto.Result = false;
                resultDto.Message = "删除失败，这不是您的文章";
                return resultDto;
            }


            resultDto = await _repositoryFactory.Default.DeleteAsync<EssayEntity>(new { Id = id }, isPhysics: true);
            return resultDto;

        }

        /// <summary>
        /// 相似列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("similarlist")]
        public async Task<ResultDto<IEnumerable<EssayEntity>>> GetEssaySimilarList(long essayId)
        {
            var resultDto = new ResultDto<IEnumerable<EssayEntity>>();
            resultDto.Result = true;
            resultDto.Data = _repositoryFactory.GetRepository<IEssayRepository>().GetEssaySimilarList(essayId);
            return await Task.FromResult(resultDto);
        }


        /// <summary>
        /// 其他列表
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("otherlist")]
        public async Task<ResultDto<IEnumerable<EssayEntity>>> GetEssayOtherList(long essayId)
        {
            var resultDto = new ResultDto<IEnumerable<EssayEntity>>();
            resultDto.Result = true;
            resultDto.Data = _repositoryFactory.GetRepository<IEssayRepository>().GetEssayOtherList(essayId);
            return await Task.FromResult(resultDto);
        }

        #endregion


        #region other
        /// <summary>
        /// 添加喜欢
        /// </summary>
        /// <param name="essayId"></param>
        /// <returns></returns>
        [HttpPost("like")]
        public async Task<ResultDto> Like(long essayId)
        {
            return await Task.FromResult(_repositoryFactory.GetRepository<IEssayLikeRepository>().ChangeEssayLike(_kardSession.UserId.Value, essayId));
        }

        /// <summary>
        /// 喜欢列表
        /// </summary>
        /// <param name="essayId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("likelist")]
        public async Task<ResultDto<IEnumerable<EssayLikeDto>>> GetLikeList(long essayId)
        {
            var resultDto = new ResultDto<IEnumerable<EssayLikeDto>>();
            resultDto.Result = true;
            resultDto.Data = _repositoryFactory.GetRepository<IEssayLikeRepository>().GetEssayLikeList(essayId);
            return await Task.FromResult(resultDto);
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="essayId"></param>
        /// <param name="content"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost("addcomment")]
        public async Task<ResultDto> AddComment(long essayId, string content, long? parentId)
        {
            var resultDto = new ResultDto();
            var essayComment = new EssayCommentEntity
            {
                EssayId = essayId,
                Content = content,
                ParentId = parentId
            };

            essayComment.AuditCreation(_kardSession.UserId.Value);
            resultDto.Result = _defaultRepository.CreateAndGetId<EssayCommentEntity, long>(essayComment).Result;
            return await Task.FromResult(resultDto);
        }

        /// <summary>
        /// 评论列表
        /// </summary>
        /// <param name="essayId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("commentlist")]
        public async Task<ResultDto<IEnumerable<EssayCommentDto>>> GetCommentList(long essayId)
        {
            var essayCommentList = _repositoryFactory.GetRepository<IEssayCommentRepository>().GetEssayCommentList(essayId) ?? new List<EssayCommentDto>();
            var pageCommentList = essayCommentList.Where((item, index) => index < 10);

            var resultDto = new ResultDto<IEnumerable<EssayCommentDto>>();
            resultDto.Result = true;
            resultDto.Data = AppendChild(pageCommentList, essayCommentList);
            return await Task.FromResult(resultDto);
        }

        private IEnumerable<EssayCommentDto> AppendChild(IEnumerable<EssayCommentDto> childList, IEnumerable<EssayCommentDto> commentList)
        {
            foreach (var child in childList)
            {
                if (child.ParentId != null && child.ParentId.HasValue)
                {
                    child.ParentCommentDtoList = commentList.Where(c => c.Id == child.ParentId);
                    child.ParentCommentDtoList = AppendChild(child.ParentCommentDtoList, commentList);
                }
            }
            return childList;
        }


        /// <summary>
        /// 微信转发
        /// </summary>
        [AllowAnonymous]
        [HttpPost("jssdk")]
        public async Task<ResultDto> JsSdk(string url)
        {
            var configSection = _configuration.GetSection("AppSetting:WeChat:Web");
            //获取时间戳
            var timestamp = JSSDKHelper.GetTimestamp();
            //获取随机码
            var nonceStr = JSSDKHelper.GetNoncestr();
            var appId = configSection.GetValue<string>("AppId");
            var appSecret = configSection.GetValue<string>("AppSecret");
            //获取票证

            var jsTicket = await JsApiTicketContainer.TryGetJsApiTicketAsync(appId, appSecret);
            //获取签名
            //nonceStr = "Wm3WZYTPz0wzccnW";
            //jsTicket = "sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg";
            //timestamp = "1414587457";
            //url = "http://mp.weixin.qq.com?params=value";
            //url = url?? Request.GetAbsoluteUri();
            var signature = JSSDKHelper.GetSignature(jsTicket, nonceStr, timestamp, url);

            var resultDto = new ResultDto
            {
                Result = true,
                Data = new { appId, timestamp, nonceStr, signature },
                Message = "查询成功"
            };

            _logger.LogDebug(Serialize.ToJson(new { url, appId, jsTicket, timestamp, nonceStr, signature }));
            return await Task.FromResult(resultDto);
        }





        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="connNum"></param>
        ///// <returns></returns>
        //[HttpGet("test")]
        //public async Task<ResultDto<string>> Test(long? connNum = 10)
        //{
        //    var milliseconds = await RunTask(connNum);
        //    _logger.LogDebug($"耗时：{milliseconds}ms");
        //    return new ResultDto<string>() { Result = true, Data = $"耗时：{milliseconds}ms" };
        //}

        //private async Task<long> RunTask(long? taskNum)
        //{
        //    var taskList = new List<Task<long>>();
        //    for (int i = 0; i < taskNum; i++)
        //    {
        //        taskList.Add(Task.RunAsync(() =>
        //        {
        //            Stopwatch sw = new Stopwatch();
        //            sw.Start();
        //            for (int j = 0; j < taskNum; j++)
        //            {
        //               await Like(taskNum.Value);
        //            }
        //            sw.Stop();
        //            return sw.ElapsedMilliseconds;
        //        }));
        //    }

        //    var result = await Task.WhenAll(taskList);
        //    return (result.Sum() / (taskNum.Value * taskNum.Value));
        //}


        ///// <summary>
        ///// 微信参数准备
        ///// </summary>
        //[HttpPost("jssdk")]
        //public async Task<object> JsSdkAsync(string url)
        //{
        //    //获取时间戳
        //    var timestamp =GetTimestamp();
        //    //获取随机码
        //    var nonceStr =  EncryptHelper.GetMD5(Guid.NewGuid().ToString(), "UTF-8");
        //    var appId = "wx109fc14b4956fc70";
        //    var appSecret = "a8e7f19d69cbde0272fd866fe7392874";
        //    //获取票证

        //    var jsTicket = await JsApiTicketContainer.TryGetJsApiTicketAsync(appId, appSecret);
        //    //获取签名
        //    //nonceStr = "Wm3WZYTPz0wzccnW";
        //    //jsTicket = "sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg";
        //    //timestamp = "1414587457";
        //    //url = "http://mp.weixin.qq.com?params=value";
        //    //url = url?? Request.GetAbsoluteUri();
        //    var signature = JSSDKHelper.GetSignature(jsTicket, nonceStr, timestamp, url);


        //    _logger.LogDebug(Serialize.ToJson(new { url, appId, jsTicket, timestamp, nonceStr, signature }));
        //    return json.ToJsonResult();
        //}


        //#endregion


        //public static async Task<AccessTokenResult> GetTokenAsync(string appid, string secret, string grant_type = "client_credential")
        //{
        //    //注意：此方法不能再使用ApiHandlerWapper.TryCommonApi()，否则会循环
        //    var url = string.Format(Config.ApiMpHost + "/cgi-bin/token?grant_type={0}&appid={1}&secret={2}",
        //                            grant_type.AsUrlData(), appid.AsUrlData(), secret.AsUrlData());

        //    AccessTokenResult result = await Get.GetJsonAsync<AccessTokenResult>(url);
        //    return result;
        //}

        //public readonly static DateTime BaseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);//Unix起始时间

        //public static string GetTimestamp()
        //{
        //    //var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    //return Convert.ToInt64(ts.TotalSeconds).ToString();
        //    var ts = GetWeixinDateTime(DateTime.Now);
        //    return ts.ToString();
        //}

        //public static long GetWeixinDateTime(DateTime dateTime)
        //{
        //    return (long)(dateTime.ToUniversalTime() - BaseTime).TotalSeconds;
        //}

        #endregion

    }

}