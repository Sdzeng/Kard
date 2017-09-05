using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface IMediaRepository : IRepository
    {
        IEnumerable<MediaEntity> GetTopMediaPicture(DateTime creationTime);
    }
}
