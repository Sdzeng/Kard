using Kard.Core.Dtos;
using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Core.IRepositories
{
    public interface ILongTaskRepository : IRepository
    {
        ResultDto AddTask(LongTaskEntity entity);
    }
}
