using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kard.Workers
{
    public class CompositeWorker : BaseWorker, IWorker
    {
        private List<IWorker> children = new List<IWorker>();

     

        public int Count
        {
            get { return children.Count; }
        }

        public void Add(IWorker c)
        {
            if (!children.Contains(c))
            {
                children.Add(c);
            }
        }

        public void Remove(IWorker c)
        {
            if (children.Contains(c))
            {
                children.Remove(c);
            }
        }

        public void Start()
        {
            foreach (var c in children)
            {
                c.Start();
            }

        }



        public void Stop()
        {
            var tasks = children.Select(c => Task.Run(() => c.Stop()));
            //WaitAll会阻塞线程，WhenAll会阻塞线程
            //这里应采用WaitAll防止服务退出时Worker的Stop还没执行完
            Task.WaitAll(tasks.ToArray());
        }




        public void Pause()
        {
            foreach (var c in children)
            {
                c.Pause();
            }

        }


        public void Resume()
        {
            foreach (var c in children)
            {
                c.Resume();
            }

        }


    }
}
