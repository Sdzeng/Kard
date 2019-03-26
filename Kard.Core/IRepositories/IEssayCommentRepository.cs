using Kard.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface IEssayCommentRepository : IRepository
    {
        IEnumerable<EssayCommentDto> GetEssayCommentList(long id);
    }
}
