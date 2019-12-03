using Dapper;
using Kard.Core.Dtos;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Kard.Dapper.Mysql.Repositories
{
    public class EssayLikeRepository : IEssayLikeRepository
    {
        private readonly IDefaultRepository _defaultRepository;
        public EssayLikeRepository(IDefaultRepository defaultRepository)
        {
            _defaultRepository = defaultRepository;
        }

        public ResultDto ChangeEssayLike(long userId, long essayId)
        {
            var resultDto = new ResultDto();

            //多处改动使用事务时则用事务级别隔离read committed加行锁
            DynamicParameters pars = new DynamicParameters();
            pars.Add("@iuserId", userId);
            pars.Add("@iessayId", essayId);
            pars.Add("@icreationTime", DateTime.Now);
            pars.Add("@olikeNum", 0, DbType.Int32, ParameterDirection.Output);
            pars.Add("@oisLike", null, DbType.Byte, ParameterDirection.Output);


            try
            {
                var ee = _defaultRepository.ConnExecute(conn => conn.Execute("changeEssayLike", pars, commandType: CommandType.StoredProcedure));//res2.Count = 80
                var likeNum = pars.Get<int>("@olikeNum");
                var isLike = pars.Get<byte>("@oisLike") == 1;

                resultDto.Result = true;
                resultDto.Data = new { LikeNum = likeNum, IsLike = isLike };
            }
            catch (Exception ex)
            {
                resultDto.Result = false;
                resultDto.Message = ex.Message;
            }
            return resultDto;
        }

        public IEnumerable<EssayLikeDto> GetEssayLikeList(long id)
        {
            string sql = @"select essayLike.Id, essayLike.EssayId,essayLike.CreationTime,kuser.NickName KuserNickName,kuser.AvatarUrl KuserAvatarUrl 
                from essayLike 
                left join kuser on essayLike.CreatorUserId=kuser.Id 
                where essayLike.EssayId=@EssayId 
               order by essayLike.CreationTime desc";

            return _defaultRepository.ConnExecute(conn => conn.Query<EssayLikeDto>(sql, new { EssayId = id }));
        }
    }
}
