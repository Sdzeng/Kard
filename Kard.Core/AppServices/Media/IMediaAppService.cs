using Kard.Core.Dtos;
using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.AppServices.Media
{
    public interface IMediaAppService : IAppService
    {
        IEnumerable<TopMediaDto> GetTopMediaPicture(DateTime creationTime);
    }
}
