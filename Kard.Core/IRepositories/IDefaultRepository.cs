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

 
        IEnumerable<TopMediaDto> GetTopMediaPicture(DateTime creationTime);

        bool IsExistUser(string name, string phone, string email);

        KuserEntity GetUser(long id);
    }

  
}
