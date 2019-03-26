using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.DI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface IEssayRepository : IRepository
    {
        IEnumerable<TopMediaDto> GetHomeMediaPictureList(string type, int pageIndex, int pageSize);

        IEnumerable<TopMediaDto> GetUserMediaPictureList(long userId, int count);

        IEnumerable<EssayDto> GetEssayList(DateTime creationTime);

        EssayDto GetEssayDto(long id, long? currentUserId);

        bool AddEssay(EssayEntity essayEntity, IEnumerable<TagEntity> tagList);

        bool UpdateBrowseNum(long id);


        IEnumerable<EssayEntity> GetEssaySimilarList(long id);

        IEnumerable<EssayEntity> GetEssayOtherList(long id);
    }
}
