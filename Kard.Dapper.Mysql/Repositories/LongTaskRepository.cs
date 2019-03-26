using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class LongTaskRepository : Repository, ILongTaskRepository
    {
        public LongTaskRepository(IConfiguration configuration, ILogger<Repository> logger) : base(configuration, logger)
        {
        }


        public ResultDto AddTask(LongTaskEntity entity)
        {
            var taskDate = entity.StartDate;
            var taskWeekDay = (int)taskDate.DayOfWeek;
            var taskWeekList = entity.Week.Split(',').Select(w => Convert.ToInt32(w));

            return TransExecute((conn, trans) =>
            {
                var createResult = new ResultDto();
                var createLongResult = conn.CreateAndGetId<LongTaskEntity, long>(entity, trans);
                if (!createLongResult.Result)
                {
                    _logger.LogError("添加长期目标失败，已撤销：" + createLongResult.Message);
                    createResult.Result = false;
                    createResult.Message = "添加长期目标失败";
                    return createResult;
                }

                var taskEntityList = new List<TaskEntity>();
                while (taskDate <= entity.EndDate)
                {
                    if (taskWeekList.Contains(taskWeekDay))
                    {
                        var taskEntity = new TaskEntity()
                        {
                            LongTaskId = createLongResult.Data,
                            TaskDate = taskDate,
                            StartTime = entity.StartTime,
                            EndTime = entity.EndTime,
                            Content = entity.Content,
                            IsRemind = entity.IsRemind,
                            IsDone = false
                        };
                        taskEntity.AuditCreation(entity.CreatorUserId.Value);
                        taskEntityList.Add(taskEntity);
                    }

                    taskDate = taskDate.AddDays(1);
                    taskWeekDay = (taskWeekDay + 1) % 7;
                }


                if (!conn.CreateList(taskEntityList, trans).Result)
                {
                    _logger.LogError("添加小目标失败，已撤销");
                    createResult.Result = false;
                    createResult.Message = "添加小目标失败";
                    return createResult;
                }

                createResult.Result = true;
                return createResult;
            });
        }
    }
}
