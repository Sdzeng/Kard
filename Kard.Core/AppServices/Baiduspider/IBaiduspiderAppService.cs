using Kard.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Core.AppServices.Baiduspider
{
    public interface IBaiduspiderAppService : IAppService
    {
        void Baiduspider(string pageUrl);
        Task<ResultDto> BaiduspiderAsync(List<string> urls = null);
    }
}
