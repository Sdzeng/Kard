using Kard.Core.Entities;
using Kard.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.AppServices.Cover
{
    public class CoverAppService: ICoverAppService
    {
        private readonly ICoverRepository _coverRepository;

        public CoverAppService(ICoverRepository covertRepository)
        {
            _coverRepository = covertRepository;

        }

        public CoverEntity GetDateCover(DateTime showDate)
        {
            return _coverRepository.GetDateCover(showDate);
        }
    }
}
