using Microsoft.Extensions.Logging;

namespace Kard.Workers
{
    public interface IWorker
    {
        ILogger Logger {  get;  }

        WorkTaskArgs TaskArgs { get; }
        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复
        /// </summary>
        void Resume();


    }
}
