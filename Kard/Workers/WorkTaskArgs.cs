 
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Workers
{
    public class WorkTaskArgs
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// 任务执行间隔，秒为单位
        /// </summary>
        public int ThreadInterval { get; set; }

        /// <summary>
        /// 任务内容
        /// </summary>
        public Func<ILogger, WorkTaskArgs, bool> TaskMethod { get; set; }
    }
}
