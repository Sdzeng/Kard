using Kard.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Kard.Core.Entities;
using System.Data;
using Dapper;
using System.Linq;

namespace Kard.Dapper.Mysql.Repositories
{
    public class MediaRepository : Repository, IMediaRepository
    {
        public MediaRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public IEnumerable<MediaEntity> GetTopMediaPicture(DateTime creationTime)
        {
            using (IDbConnection connecton = GetConnection())
            {

                string sql = @"select t.*
                from(select * from media where CreationTime >@CreationTime and MediaType = 'picture' order by HeartNum desc) t
                 group by t.EssayId limit 4";
                var mediaEntity = connecton.Query<MediaEntity>(sql, new { CreationTime = creationTime });

                return mediaEntity;
            }
        }

    }
}
