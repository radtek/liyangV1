using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace WCAPP.Utils
{
    public class TaskQueue
    {
        Queue<Task> tasks = new Queue<Task>();

        Thread thread;
        Semaphore eventSemaphore = new Semaphore(0, Int32.MaxValue);

        public TaskQueue()
        {
            thread = new Thread(new ThreadStart(TaskThread));
            thread.Start();
        }

        public TResult RunTask<TResult>(Func<TResult> func)
        {
            Func<object> func1 = () => { return func(); };
            var task = new Task(func1);
            lock (tasks) tasks.Enqueue(task);
            eventSemaphore.Release();
            return (TResult) task.WaitResult();
        }

        public void RunTask(Action func)
        {
            Func<object> func1 = () =>
            {
                func();
                return "";
            };
            var task = new Task(func1);
            lock (tasks) tasks.Enqueue(task);
            eventSemaphore.Release();
            task.WaitResult();
        }

        public void PostTask(Action func)
        {
            Func<object> func1 = () =>
            {
                func();
                return "";
            };
            var task = new Task(func1);
            lock (tasks) tasks.Enqueue(task);
            eventSemaphore.Release();
        }

        private void TaskThread()
        {
            while (true)
            {
                eventSemaphore.WaitOne();

                Task task;
                lock (tasks) task = tasks.Dequeue();
                task.Run();
            }
        }
    }

    class Task
    {
        object result;

        public Func<object> Function { get; private set; }
        public ManualResetEvent Finished { get; set; }

        public Task(Func<object> func)
        {
            Function = func;
            Finished = new ManualResetEvent(false);
        }

        public void Run()
        {
            result = Function();
            Finished.Set();
        }

        public object WaitResult()
        {
            Finished.WaitOne();
            return result;
        }
    }
}