using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Reflection;

public class MyThreadPool {

    static int numThreads;
    static PoolThread[] workers;

    static Task[] threadTasks;
    public static List<Task> taskQueue = new List<Task>();

    public static readonly object queueLocker = new object();

    public static void StartThreadPool()
    {
        Initialize();
        Run();
    }

    public static Task AddActionToQueue(WaitCallback callback, object args)
    {

        Task task = new Task(callback, args);
        lock (queueLocker)
            taskQueue.Add(task);

        for(int i = 0; i < workers.Length; i++)
        {
            lock (workers[i].eventLocker)
            {
                workers[i].threadCanStart.Set();
            }
        }
        return task;
    }

    static void Initialize()
    {
        numThreads = SystemInfo.processorCount-1;
        workers = new PoolThread[numThreads];
        threadTasks = new Task[numThreads];
    }

    static void Run()
    {

        for (int i = 0; i < numThreads; i++)
        {
            int threadNum = i;
            workers[i] = new PoolThread();
        }

    }
}


public class Task
{
    WaitCallback callback;
    object args;
    public bool isRunning = false;
    public bool isDone = false;

    public Task(WaitCallback callback, object args)
    {
        this.callback = callback;
        this.args = args;
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
    Thread thread;
    int threadNum;
    public ManualResetEvent threadCanStart;

    public readonly object eventLocker = new object();

    public PoolThread()
    {
        threadCanStart = new ManualResetEvent(true);
        thread = new Thread(delegate () { Worker(); });
        thread.Priority = System.Threading.ThreadPriority.Lowest;
        thread.IsBackground = true;

        thread.Start();
    }

    public void GetTask()
    {
        if (MyThreadPool.taskQueue.Count > 0)
        {
            lock (MyThreadPool.queueLocker)
            {
                task = MyThreadPool.taskQueue[0];
                MyThreadPool.taskQueue.RemoveAt(0);
            }
        }
        else
        {
            task = null;
            lock (eventLocker)
                threadCanStart.Reset();
        }
    }

    void Worker()
    {
        while (true)
        {
            threadCanStart.WaitOne();
            GetTask();
            if (task != null)
            {
                task.RunTask();
                task.isDone = true;
            }
        }
    }
}

