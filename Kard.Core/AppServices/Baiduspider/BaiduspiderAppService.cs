using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Core.AppServices.Baiduspider
{
    public class BaiduspiderAppService : IBaiduspiderAppService
    {
        private readonly IDefaultRepository _defaultRepository;
        public BaiduspiderAppService(IDefaultRepository defaultRepository) {
            _defaultRepository = defaultRepository;
        }
        public async Task<ResultDto> BaiduspiderAsync(List<string> urls=null)
        {
            if (urls == null || urls.Count() <= 0) {
               var pageUrls=  _defaultRepository.Query<string>(@"select PageUrl 
               from essay where IsDeleted=0 and IsPublish=1 and (CreationTime>=@CreationTime or LastModificationTime>=@LastModificationTime) order by id desc",
               new { CreationTime =DateTime.Now.AddDays(-1),LastModificationTime = DateTime.Now.AddDays(-1)});
                urls = pageUrls.Select(item =>$"http://www.coretn.cn/{item}").ToList();
                urls.AddRange(pageUrls.Select(item => ($"http://www.coretn.cn/{item}").Replace("essay/","essay/m/")));
            }
            urls.Add("http://www.coretn.cn/home.html");
            urls.Add("http://www.coretn.cn/search.html");
            urls.Add("http://www.coretn.cn/search.html?keyword=人工智能");
            urls.Add("http://www.coretn.cn/search.html?keyword=Python");
            urls.Add("http://www.coretn.cn/search.html?keyword=AI");
            urls.Add("http://www.coretn.cn/search.html?keyword=C++");
            urls.Add("http://www.coretn.cn/search.html?keyword=.Net%20Core");
            urls.Add("http://www.coretn.cn/search.html?keyword=微信");
            urls.Add("http://www.coretn.cn/search.html?keyword=JQuery");
            urls.Add("http://www.coretn.cn/search.html?keyword=JavaScript");
            urls.Add("http://www.coretn.cn/search.html?keyword=Html");

            var pars = string.Join("\n", urls);
            string postResult = Utils.HttpPost("http://data.zz.baidu.com/urls?site=www.coretn.cn&token=ZxOJuNTh1YFbQF1m", pars, "'text/plain");


            return await Task.FromResult(new ResultDto { Result = true, Data = postResult });
        }
    }
}
