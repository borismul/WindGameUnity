using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Reflection;

public enum TaskPriority { low, medium, high };

public class MyThreadPool
{

    static int numThreads;
    public static PoolThread[] workers;
    public static bool abort = false;
    public static List<Task> lowTaskQueue = new List<Task>();
    public static List<Task> mediumTaskQueue = new List<Task>();
    public static List<Task> highTaskQueue = new List<Task>();

    public static int waitingThreads = 0;
    public static readonly object queueLocker = new object();

    public static readonly object waitingThreadsLocker = new object();


    public static void StartThreadPool(int NumThreads)
    {
        Initialize(NumThreads);
        abort = false;
        Run();
    }

    public static Task AddActionToQueue(WaitCallback callback, object args, TaskPriority priority = TaskPriority.low)
    {

        Task task = new Task(callback, args, priority);
        lock (queueLocker)
        {
            if (priority == TaskPriority.low)
                lowTaskQueue.Add(task);
            else if (priority == TaskPriority.medium)
                mediumTaskQueue.Add(task);
            else
                highTaskQueue.Add(task);

        }

        for (int i = 0; i < workers.Length; i++)
        {
            lock (workers[i].eventLocker)
            {
                workers[i].threadCanStart.Set();
            }
        }
        return task;
    }

    static void Initialize(int NumThreads)
    {
        numThreads = NumThreads;
        workers = new PoolThread[numThreads];
    }

    static void Run()
    {

        for (int i = 0; i < numThreads; i++)
        {
            int threadNum = i;
            workers[i] = new PoolThread(threadNum);
        }

    }

    public static void DestroyThreadPool()
    {
        abort = true;
        waitingThreads = 0;
    }

    public static int GetWaitingThreads()
    {
        lock (waitingThreadsLocker)
            return waitingThreads;
    }
}


public class Task
{
    WaitCallback callback;
    object args;
    public bool isRunning = false;
    public bool isDone = false;
    TaskPriority priority;

    public Task(WaitCallback callback, object args, TaskPriority priority)
    {
        this.callback = callback;
        this.args = args;
        this.priority = priority;
    }

    public void RunTask()
    {
        try
        {
            callback(args);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

    }
}

public class PoolThread
{
    public Task task;
    public Thread thread;
    int threadNum;
    public ManualResetEvent threadCanStart;

    public readonly object eventLocker = new object();

    public PoolThread(int threadNum)
    {
        threadCanStart = new ManualResetEvent(true);
        thread = new Thread(delegate () { Worker(); });
        thread.Priority = System.Threading.ThreadPriority.Lowest;
        thread.IsBackground = true;
        this.threadNum = threadNum;
        thread.Start();
    }

    public void GetTask()
    {
        lock (MyThreadPool.queueLocker)
        {
            if (MyThreadPool.highTaskQueue.Count > 0)
            {
                task = MyThreadPool.highTaskQueue[0];
                MyThreadPool.highTaskQueue.RemoveAt(0);
            }
            else if(MyThreadPool.mediumTaskQueue.Count > 0)
            {
                task = MyThreadPool.mediumTaskQueue[0];
                MyThreadPool.mediumTaskQueue.RemoveAt(0);
            }
            else if (MyThreadPool.lowTaskQueue.Count > 0)
            {
                task = MyThreadPool.lowTaskQueue[0];
                MyThreadPool.lowTaskQueue.RemoveAt(0);
            }
            else
            {
                task = null;
                lock (eventLocker)
                    threadCanStart.Reset();
            }
        }

    }

    void SetWaitingThreads(bool plus)
    {
        lock (MyThreadPool.waitingThreadsLocker)
        {
            if (plus)
                MyThreadPool.waitingThreads++;

            else
                MyThreadPool.waitingThreads--;
        }
    }

    void Worker()
    {
        while (true)
        {
            SetWaitingThreads(true);
            threadCanStart.WaitOne();
            if (MyThreadPool.abort)
                thread.Abort();
            SetWaitingThreads(false);



            GetTask();
            if (task != null)
            {
                task.RunTask();
                task.isDone = true;
            }


        }
    }

}

