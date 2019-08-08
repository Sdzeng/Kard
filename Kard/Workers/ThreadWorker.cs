using Microsoft.Extensions.Logging;
using System;
using System.Threading;


namespace Kard.Workers
{
    public class ThreadWorker : BaseWorker, IWorker
    {
        
        private readonly Thread _startThread;
        private readonly object _lockObj = new object();
        //0：false 1：true
        private int _isPause;
        private int _isStop;
       

        public ThreadWorker(ILogger<ThreadWorker> logger, WorkTaskArgs taskArgs):base(logger,taskArgs)
        {
            //IsBackground
            //false 默认，当主线程退出的时候,线程还会继续执行下去，直到线程执行结束，
            //         但window服务会最多等待30秒后强制关闭。
            //true;随着主线程的退出而退出
            _startThread = new Thread(new ParameterizedThreadStart(StartThreadTask));
            Interlocked.Exchange(ref _isPause, 0);
            Interlocked.Exchange(ref _isStop, 0);
        }

        public void Start()
        {
            try
            {
                _startThread.Start(TaskArgs);

                Logger.LogInformation($"{TaskArgs.WorkName}启动成功");
            }
            catch (Exception ex)
            {
                Logger.LogError($"{TaskArgs.WorkName}启动失败", ex);
            }
        }


        private void StartThreadTask(object obj)
        {
            var taskArgs = obj as WorkTaskArgs;
            while (_isStop.Equals(0))
            {
                if (_isPause.Equals(1))
                {
                    Thread.Sleep(1000 * taskArgs.ThreadInterval);
                    continue;
                }

               //执行任务
                lock (_lockObj)
                {
                    try
                    {
                        if (!taskArgs.TaskMethod.Invoke(Logger,taskArgs)) {
                            Logger.LogError($"{taskArgs.WorkName}任务执行失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"{taskArgs.WorkName}任务执行异常", ex);
                    }
                    finally
                    {
                       Logger.LogInformation($"{taskArgs.WorkName}任务执行完成");
                    }
                }

                //节省线程停止时使用的时间
                if (_isStop.Equals(0))
                {
                    Thread.Sleep(1000 * taskArgs.ThreadInterval);
                    //Interlocked.CompareExchange(ref _lockObj, 0, 1);
                }
            }

            //Logger.Debug("后台线程停止");
        }





        public void Pause()
        {
            Interlocked.Exchange(ref _isPause, 1);
           Logger.LogInformation($"{TaskArgs.WorkName}暂停运行");
        }

        public void Resume()
        {
            Interlocked.Exchange(ref _isPause, 0);
           Logger.LogInformation($"{TaskArgs.WorkName}恢复运行");
        }

        #region 备用 Stop
        ///// <summary>
        ///// 优点：确保数据原子性
        ///// 缺点：window服务停止时间长，必须等待任务执行完成，
        /////而且期间会因window服务的30秒超时被强制关闭
        ///// </summary>
        //public void Stop()
        //{
        //    try
        //    {
        //        //开始停止线程
        //        //如业务代码在运行中，则等待业务代码执行完成后再停止服务
        //        //而后台线程最多运行一个休眠周期后停止
        //        _isStop = true;
        //       Logger.LogInformation("停止成功，后台线程则最多运行一个休眠周期后停止");

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.ErrorEmail("停止出现异常", ex);
        //    }
        //}

        ///// <summary>
        ///// 优点：确保数据原子性，window服务停止时间短
        ///// 缺点：期间会因window服务的30秒超时被强制关闭
        ///// </summary>
        //public void Stop()
        //{
        //    try
        //    {
        //       Logger.LogInformation("开始停止");
        //        _stopThread.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.ErrorEmail("停止出现异常", ex);
        //    }
        //}


        //private void StopThreadTask()
        //{
        //    try
        //    {
        //        Interlocked.Exchange(ref _isStop, 1);
        //        while (true)
        //        {
        //            if (_startThread.IsAlive)
        //            {
        //               Logger.LogInformation(string.Concat("任务未执行完，等待", AppConfig.ThreadWaitInterval.ToString(), "秒"));
        //                Thread.Sleep(1000 * AppConfig.ThreadWaitInterval);
        //            }
        //            else
        //            {
        //               Logger.LogInformation("停止成功");
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.ErrorEmail("辅助停止出现异常", ex);
        //    }
        //}
        #endregion

        /// <summary>
        /// 优点：确保数据原子性，window服务停止时间短，提供友善方式和强制方式
        /// 缺点：暂无
        /// </summary>
        public void Stop()
        {
            try
            {
                #region  采用友善方式退出
                Interlocked.Exchange(ref _isStop, 1);
                int i = 2;
                while (i > 0)
                {
                    if (_startThread.IsAlive)
                    {
                       Logger.LogInformation($"{TaskArgs.WorkName}任务未执行完，等待5秒");
                        Thread.Sleep(1000 * 2);
                        i--;
                    }
                    else
                    {
                       Logger.LogInformation($"{TaskArgs.WorkName}停止成功");
                        return;
                    }
                }
                #endregion

                #region  采用强制方式退出
               Logger.LogInformation($"{TaskArgs.WorkName}等待业务代码执行完");
                lock (_lockObj)
                {
                    //不在业务代码块，可强制退出
                    _startThread.Abort();
                   Logger.LogInformation($"{TaskArgs.WorkName}停止成功");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.LogError($"{TaskArgs.WorkName}停止出现异常", ex);
            }
        }







    }
}
