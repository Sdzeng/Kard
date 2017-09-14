using Kard.Core.Dtos;
using System;
using System.Collections.Generic;

namespace Kard.Core.IRepositories
{
    public interface IMediaRepository : IRepository
    {
        IEnumerable<TopMediaDto> GetTopMediaPicture(DateTime creationTime);
    }
}
