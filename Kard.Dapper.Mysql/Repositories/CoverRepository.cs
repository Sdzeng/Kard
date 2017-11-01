using Dapper;
using Kard.Core.Entities;
using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;

namespace Kard.Dapper.Mysql.Repositories
{
    public class CoverRepository : Repository, ICoverRepository
    {

        public CoverRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public CoverEntity GetDateCover(DateTime showDate)
        {
            return ConnExecute(connecton =>
            {
                string sql = @"select 
                                cover.*,
	                            media.*,
                                essay.*,
		                        kuser.*   
		                        from 
		                        (select * from cover where showdate <= @ShowDate order by showdate desc limit 1 ) cover
		                        left join media on cover.mediaid = media.id   
		                        left join essay on media.essayid = essay.id 
                                left join kuser on essay.creator=kuser.id 
                                where essay.isdeleted=0 and media.mediatype='picture' ";
                var entityList = connecton.Query<CoverEntity, MediaEntity, EssayEntity, KuserEntity, CoverEntity>(sql, (cover, media, essay, kuser) =>
                  {
                      media.Essay = essay;
                      media.Kuser = kuser;
                      cover.Media = media;
                      return cover;
                  },
                  new { ShowDate = showDate },
                  splitOn: "Id");

                if (entityList != null && entityList.Any())
                {
                    return entityList.First();
                }

                return null;
            });
        }




    }
}
