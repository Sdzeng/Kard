using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.DI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface ICoverRepository : IRepository
    {

        CoverDto GetDateCover(DateTime showDate);
    }
}
