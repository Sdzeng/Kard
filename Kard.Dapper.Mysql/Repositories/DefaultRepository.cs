using Kard.Core.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kard.Dapper.Mysql.Repositories
{
    public class DefaultRepository : Repository, IDefaultRepository
    {


        public DefaultRepository(
            IConfiguration configuration,
            ILogger<DefaultRepository> logger,
            ICoverRepository cover,
            IEssayCommentRepository essayComment,
            IEssayLikeRepository essayLike,
            IEssayRepository essay,
            IKuserRepository kuser,
            ILongTaskRepository longTask
            ) : base(configuration, logger)
        {
            Cover = cover;
            EssayComment = essayComment;
            EssayLike = essayLike;
            Essay = essay;
            Kuser = kuser;
            LongTask = longTask;
        }


        public ICoverRepository Cover { get; }

        public IEssayCommentRepository EssayComment { get; }

        public IEssayLikeRepository EssayLike { get; }

        public IEssayRepository Essay { get; }


        public IKuserRepository Kuser { get; }

        public ILongTaskRepository LongTask { get; }



    }
}
