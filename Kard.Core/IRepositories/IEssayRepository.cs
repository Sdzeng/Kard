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
        IEnumerable<TopMediaDto> GetHomeMediaPictureList(string keyword, int pageIndex, int pageSize,string orderBy);

        IEnumerable<object> GetUserNews(long userId, int pageIndex, int pageSize, string orderBy);

        IEnumerable<EssayDto> GetEssayList(DateTime creationTime);

        EssayDto GetEssayDto(long id, long? currentUserId);

        ResultDto<long> AddEssay(EssayEntity essayEntity, IEnumerable<TagEntity> tagList);

        bool UpdateEssay(EssayEntity essayEntity, IEnumerable<TagEntity> tagList);

        bool UpdateBrowseNum(long id);


        IEnumerable<EssayEntity> GetEssaySimilarList(long id);

        IEnumerable<EssayEntity> GetEssayOtherList(long id);
    }
}
