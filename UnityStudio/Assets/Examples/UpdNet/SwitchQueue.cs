using System.Collections.Generic;
using System.Threading;

namespace VRNetLibrary
{
    /// <summary>
    /// 切换队列，一个写入，一个读取，保持写入读取异步(注:并没有做多线程验证)。
    /// </summary>
    public class SwitchQueue<T>
    {
        private Queue<T> writeQueue;

        private Queue<T> readQueue;

        public SwitchQueue()
            : this(16)
        {
        }

        public SwitchQueue(int capacity)
        {
            this.writeQueue = new Queue<T>(capacity);
            this.readQueue = new Queue<T>(capacity);
        }

        public void Clear()
        {
            Queue<T> obj = this.writeQueue;
            Monitor.Enter(obj);
            try
            {
                Queue<T> obj2 = this.writeQueue;
                Monitor.Enter(obj2);
                try
                {
                    this.writeQueue.Clear();
                    this.readQueue.Clear();
                }
                finally
                {
                    Monitor.Exit(obj2);
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
        }

        public T Dequeue()
        {
            T val = default(T);
            Queue<T> obj = this.readQueue;
            Monitor.Enter(obj);
            try
            {
                return this.readQueue.Dequeue();
            }
            finally
            {
                Monitor.Exit(obj);
            }
        }

        public void Enqueue(T obj)
        {
            Queue<T> obj2 = this.writeQueue;
            Monitor.Enter(obj2);
            try
            {
                this.writeQueue.Enqueue(obj);
            }
            finally
            {
                Monitor.Exit(obj2);
            }
        }

        public void Switch()
        {
            Queue<T> obj = this.writeQueue;
            Monitor.Enter(obj);
            try
            {
                Queue<T> obj2 = this.readQueue;
                Monitor.Enter(obj2);
                try
                {
                    Queue<T> queue = this.writeQueue;
                    this.writeQueue = this.readQueue;
                    this.readQueue = queue;
                }
                finally
                {
                    Monitor.Exit(obj2);
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
        }

        public bool Empty()
        {
            return this.readQueue.Count == 0;
        }
    }
}
