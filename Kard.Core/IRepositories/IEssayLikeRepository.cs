using Kard.Core.Dtos;
using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface IEssayLikeRepository : IRepository
    {

        ResultDto ChangeEssayLike(long userId, long essayId);

        IEnumerable<EssayLikeEntity> GetEssayLikeList(long id);
    }
}
