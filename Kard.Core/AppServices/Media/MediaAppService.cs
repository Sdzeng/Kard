using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.AppServices.Media
{
    public class MediaAppService: IMediaAppService
    {
        private readonly IMediaRepository _mediaRepository;

        public MediaAppService(IMediaRepository mediaRepository)
        {
            _mediaRepository = mediaRepository;

        }

        public IEnumerable<TopMediaDto> GetTopMediaPicture(DateTime creationTime)
        {
            return _mediaRepository.GetTopMediaPicture(creationTime);
        }
    }
}
