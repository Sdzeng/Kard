
using DapperExtensions.Mapper;
using Kard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Dapper.Mysql.Mapper
{
    public class KuserMapper : ClassMapper<KuserEntity>
    {

        public KuserMapper()
        {
            Table("kuser");
            Map(e => e.AuthenticationType).Ignore();
            Map(e => e.IsAuthenticated).Ignore();
            AutoMap();
        }
    }

    //public class EssayCommentMapper : ClassMapper<EssayCommentEntity>
    //{

    //    public EssayCommentMapper()
    //    {
    //        Table("essayComment");
    //        //Map(e => e.Kuser).Ignore();
    //        //Map(e => e.EssayCommentEntityList).Ignore();
    //        AutoMap();
    //    }
    //}

    public class LongTaskMapper : ClassMapper<LongTaskEntity>
    {

        public LongTaskMapper()
        {
            Table("longTask");
            Map(e => e.IsLongTerm).Ignore();
            Map(e => e.TaskDate).Ignore();
            AutoMap();
        }
    }

}
