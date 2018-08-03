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

        IEnumerable<KuserEntity> GetExistUser(string name, string phone, string nickName);

        //bool CreateAccountUser(KuserEntity user);

        IEnumerable<EssayEntity> GetEssayList(DateTime creationTime);

        EssayEntity GetEssay(long id, long? currentUserId);

        EssayEntity GetEssay2(long id);

        bool AddEssay(EssayEntity essayEntity, IEnumerable<TagEntity> tagList, IEnumerable<MediaEntity> mediaList);

        bool UpdateBrowseNum(long id);

        ResultDto ChangeEssayLike(long userId, long essayId);

        IEnumerable<EssayLikeEntity> GetEssayLikeList(long id);

        IEnumerable<EssayEntity> GetEssaySimilarList(long id);

        IEnumerable<EssayEntity> GetEssayOtherList(long id);

        //IEnumerable<EssayCommentDto> GetRootEssayCommentList(long id);

        IEnumerable<EssayCommentDto> GetEssayCommentList(long id);

        ResultDto AddTask(LongTaskEntity entity);
    }
}

  
 