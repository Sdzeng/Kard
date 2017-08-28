using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface ICoverRepository: IRepository
    {
        CoverEntity GetDateCover(DateTime showDate);
    }
}
