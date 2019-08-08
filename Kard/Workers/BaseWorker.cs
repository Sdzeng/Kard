using Microsoft.Extensions.Logging;

namespace Kard.Workers
{

    public abstract class BaseWorker
    {

        //protected static readonly string CurrentPath = Path.Combine(Environment.CurrentDirectory,"Mail\\");
        public BaseWorker()
        { }

        public BaseWorker(ILogger<BaseWorker> logger,WorkTaskArgs taskArgs)
        {
            //var type = GetType();
            Logger = logger;
            TaskArgs = taskArgs;
        }

     

        public ILogger Logger{ get; private set; }

        public WorkTaskArgs TaskArgs { get; private set; }
    }
}
