using iTOP.Enterprise.Tools.Helper;
using iTOP.Enterprise.Tools.Init;
using System;
using System.IO;
using System.Timers;

namespace Kard.Workers
{
    public class TimerWorker : BaseWorker, IWorker
    {
        private static readonly TransWorkConfig _workConfig = InitConfig.Instance.getTransConfig();
        private static readonly DirectoryInfo _sendDir = MailHelper.CreateDirectory(_workConfig.SendPath);
        private static readonly DirectoryInfo _errorFormatDir = MailHelper.CreateDirectory(_workConfig.SendErrorFormatPath);
        private static int _interlockInt = 0;
        private readonly Timer _timer;
        private bool _isPause;

        public TimerWorker()
        {
            _timer = new Timer(1);//设置间隔时间
            _timer.Elapsed += new ElapsedEventHandler(this.TimerTask); //到达时间的时候执行事件
            _timer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)
            //_timer.Enabled = true;//Start时是否立即执行System.Timers.Timer.Elapsed事件
            _isPause = false;
        }

        public void Start()
        {
            try
            {
                if (_workConfig.SendMailType != "1" && _workConfig.SendMailType != "2" && _workConfig.SendMailType != "3")
                {
                    throw new Exception(string.Format("配置文件的SendMailType值{0}有误", _workConfig.SendMailType));
                }

                //内部调用_timer.Enabled = true;是否执行System.Timers.Timer.Elapsed事件
                _timer.Start();

               Logger.LogInformation("启动成功");
            }
            catch (Exception ex)
            {
                Logger.ErrorEmail("启动失败", ex);
            }

        }




        public void Pause()
        {
            _isPause = true;
           Logger.LogInformation("暂停运行");
        }

        public void Resume()
        {
            _isPause = false;
           Logger.LogInformation("恢复运行");
        }


        public void Stop()
        {
            try
            {
                //内部调用_timer.Enabled =false;是否执行System.Timers.Timer.Elapsed事件
                _timer.Stop();
                _timer.Close();
                _timer.Dispose();
               Logger.LogInformation("停止成功");

            }
            catch (Exception ex)
            {
                Logger.ErrorEmail("停止出现异常", ex);
            }
        }


        private void TimerTask(object sender, ElapsedEventArgs args)
        {
            #region 如果是第一次执行，重置间隔时间
            var timer = sender as Timer;
            if (timer.Interval == 1)
            {
                timer.Interval = 1000 * _workConfig.ThreadInterval; 
            }
            #endregion

            if (_isPause)
            {
                return;
            }

            //发邮件
            //原子锁，当业务代码被占用时，其他线程请求运行业务代码则跳过
            if (System.Threading.Interlocked.CompareExchange(ref _interlockInt, 1, 0).Equals(0))
            {
                try
                {
                    Logger.Debug("搜索数据包");
                    MailHelper.SendMail(Logger, _sendDir, _errorFormatDir);
                }
                catch (Exception ex)
                {
                    Logger.ErrorEmail("发送邮件出错", ex);
                }
                finally
                {
                    Logger.Debug("完成搜索数据包");
                    System.Threading.Interlocked.CompareExchange(ref _interlockInt, 0, 1);
                }
            }
        }

    }
}
