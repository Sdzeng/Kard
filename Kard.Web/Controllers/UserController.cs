using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Kard.Runtime.Security.Authentication.WeChat;
using Kard.Runtime.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Kard.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = WeChatAppDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("user")]
    public class UserController : BaseController
    {
        private readonly IHostingEnvironment _env;

        private readonly IDefaultRepository _defaultRepository;
        public UserController(IHostingEnvironment env,
            ILogger<UserController> logger,
            IMemoryCache memoryCache,
            IDefaultRepository defaultRepository,
            IKardSession kardSession)
            : base(logger, memoryCache, kardSession)
        {
            //HttpContext.Session.SetString("UserId", user.Id.ToString());
            _env = env;
            _defaultRepository = defaultRepository;
        }

        //[Authorize(Roles = "member", AuthenticationSchemes= "members")]





        #region user





        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public ResultDto<KuserEntity> Index(long userId)
        {
            var resultDto = new ResultDto<KuserEntity>() { Result = true, Data = GetUser(userId) };
            return resultDto;
        }

        /// <summary>
        /// 获取用户封面
        /// </summary>
        /// <returns></returns>
        [HttpGet("cover")]
        public ResultDto<KuserEntity> GetCover()
        {
            var resultDto = new ResultDto<KuserEntity>() { Result = true, Data = GetUser(_kardSession.UserId.Value) };
            return resultDto;
        }


        private KuserEntity GetUser(long userId)
        {
            //string cacheKey = $"user[{userId}]";
            //KuserEntity kuserEntity = _memoryCache.GetOrCreate(cacheKey, (cacheEntry) =>
            //{
            //    cacheEntry.SetAbsoluteExpiration(DateTime.Now.Date.AddDays(60));

            //    return _defaultRepository.FirstOrDefault<KuserEntity>(userId);
            //});
            //return kuserEntity;

            return _defaultRepository.FirstOrDefault<KuserEntity>(userId);
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="avathorFlie"></param>
        /// <returns></returns>
        [HttpPost("uploadavathor")]
        //[Consumes("multipart/form-data")]
        //[RequestSizeLimit(100_000_000)]
        public ResultDto UploadAvatar(IFormFile avathorFlie)
        {
            var result = new ResultDto();
            if (avathorFlie == null && Request.Form.Files.Any())
            {
                avathorFlie = Request.Form.Files[0];
            }
            if (avathorFlie == null)
            {
                result.Result = true;
                result.Message = "未选择文件";
                return result;
            }
    
            string newFolder = Path.Combine("user", _kardSession.UserId.ToString());
            string newPath = Path.Combine(_env.WebRootPath, newFolder);
            string fileName = "avatar";
            string fileExtension = ".jpg"; // Path.GetExtension(ContentDispositionHeaderValue.Parse(mediaFlie.ContentDisposition).FileName.Trim('"'));
            var directoryInfo = new DirectoryInfo(newPath);
            if (directoryInfo.Exists)
            {
                var files = directoryInfo.GetFiles().Where(file=>file.Name.Contains(fileName));
                foreach (var file in files)
                {
                    file.Delete();
                }
            }
            else
            {
                directoryInfo.Create();
            }

            if (avathorFlie.Length<= 0)
            {
                result.Result = false;
                result.Message = "上传失败";
                return result;
            }

            string fullPath = Path.Combine(newPath, fileName + fileExtension);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                avathorFlie.CopyTo(stream);
            }

            fileExtension = $"{fileExtension}?v={ DateTime.Now.ToString("ddHHmmssffff")}";
            var kuser = _defaultRepository.FirstOrDefault<KuserEntity>(_kardSession.UserId.Value);
            kuser.AvatarUrl = Path.Combine(newFolder, fileName+fileExtension);
            kuser.AuditLastModification(_kardSession.UserId.Value);
            result = _defaultRepository.Update(kuser);
            if (result.Result)
            {
                result.Data = new { FileUrl = Path.Combine(newFolder, fileName), FileExtension = fileExtension };
                _memoryCache.Remove($"user[{_kardSession.UserId.Value}]");
            }
            return result;

        }



        /// <summary>
        /// 获取动态
        /// </summary>
        /// <returns></returns>
        [HttpGet("news")]
        public ResultDto GetNews(int pageIndex, int pageSize)
        {
            var resultDto = new ResultDto();
            var newsList = _defaultRepository.Essay.GetUserNews(_kardSession.UserId.Value, pageIndex, pageSize + 1, "t.CreationTime desc");
            var hasNextPage = newsList.Count() > pageSize;
     
            resultDto.Result = true;
            resultDto.Data = new
            {
                hasNextPage,
                newsList = hasNextPage ? newsList.SkipLast(1) : newsList
            };

            return resultDto;
        }





        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[Route("common/uploadimg")]
        //public async Task<IActionResult> UploadImg(IFormFile formFile)
        //{
        //    var json = new JsonResultModel<object>()
        //    {
        //        code = "200",
        //        message = "上传成功"
        //    };
        //    if (formFile == null) formFile = Request.Form.Files[0];

        //    if (formFile == null || formFile.Length <= 0)
        //    {
        //        json.code = "400";
        //        json.message = "上传失败，文件不存在";
        //        return await Task.FromResult(json.ToJsonResult());
        //    };

        //    if (!formFile.ContentType.Contains("image"))
        //    {
        //        json.code = "400";
        //        json.message = "上传失败，这不是图片文件";
        //        return await Task.FromResult(json.ToJsonResult());
        //    }

        //    if (formFile.Length > 3 * 1024 * 1024)
        //    {
        //        json.code = "400";
        //        json.message = "上传失败，文件不能大于3MB";
        //        return await Task.FromResult(json.ToJsonResult());
        //    }


        //    var dateStr = DateTime.Now.ToString("yyyyMMdd");
        //    var filePath = $"{_appConfig.ApplicationConfig.UploadConfig.ImageUploadUrl}/{dateStr}/";
        //    if (!Directory.Exists(filePath))
        //    {
        //        Directory.CreateDirectory(filePath);//不存在就创建目录 
        //    }

        //    var fileName = $"{Utils.GetRamCode()}{Path.GetExtension(formFile.FileName)}";

        //    using (var fs = System.IO.File.Create(string.Concat(filePath, fileName)))
        //    {
        //        await formFile.CopyToAsync(fs);
        //        await fs.FlushAsync();
        //        json.data = $"{_appConfig.ApplicationConfig.UploadConfig.ImageDownloadUrl}/{dateStr}/{fileName}";
        //    }

        //    return await Task.FromResult(json.ToJsonResult());
        //}





        /// <summary>
        /// 未登录返回结果
        /// </summary>
        /// <returns></returns>

        [HttpGet("notlogin")]
        [AllowAnonymous]
        public IActionResult NotLogin()
        {
            var rs = new JsonResult(new
            {
                message = "您未登录不能查看该内容"

            });

            rs.StatusCode = 401;
            return rs;
        }
        #endregion
    }
}