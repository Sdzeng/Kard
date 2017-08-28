using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.AppServices.Cover
{
    public interface ICoverAppService: IAppService
    {
        CoverEntity GetDateCover(DateTime date);
    }
}
