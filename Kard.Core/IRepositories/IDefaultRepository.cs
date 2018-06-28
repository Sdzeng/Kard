using Kard.Core.Dtos;
using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface IDefaultRepository: IRepository
    {
        CoverEntity GetDateCover(DateTime showDate);

 
        IEnumerable<TopMediaDto> GetHomeMediaPictureList(int count, string type);

        IEnumerable<TopMediaDto> GetUserMediaPictureList(long userId, int count);

        bool IsExistUser(string name, string phone, string email);

        //KuserEntity GetUser(long id);

        IEnumerable<EssayEntity> GetEssayList(DateTime creationTime);

        EssayEntity GetEssay(long id);

        EssayEntity GetEssay2(long id);

        bool AddEssay(EssayEntity essayEntity, IEnumerable<TagEntity> tagList, IEnumerable<MediaEntity> mediaList);

        bool UpdateBrowseNum(long id);

        bool ChangeEssayLike(long userId, long essayId, bool isLike);

        ResultDto AddTask(LongTaskEntity entity);
    }
}

  
 