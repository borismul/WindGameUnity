using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Reflection;

public class MyThreadPool {

    static int numThreads;
    static PoolThread[] workers;
    static ManualResetEvent[] threadIsDone;
    static ManualResetEvent[] threadCanStart;

    static Task[] threadTasks;
    static List<Task> taskQueue = new List<Task>();
    static List<Task> finishedTaskQueue = new List<Task>();

    static Thread schedulerThread;
    static ManualResetEvent schedulerEvent;

    static readonly object queueLocker = new object();

    public static void StartThreadPool()
    {
        Initialize();
        Run();
    }

    public static void AddActionToQueue(WaitCallback callback, object args)
    {

        lock (queueLocker)
            taskQueue.Add(new Task(callback, args, 0));

        schedulerEvent.Set();
    }

    static void Initialize()
    {
        numThreads = Math.Max(4, 1);
        threadIsDone = new ManualResetEvent[numThreads];
        threadCanStart = new ManualResetEvent[numThreads];
        workers = new PoolThread[numThreads];
        threadTasks = new Task[numThreads];

        schedulerEvent = new ManualResetEvent(false);
    }

    static void Run()
    {
        schedulerThread = new Thread(delegate () { Scheduler(); });
        schedulerThread.Start();
        for (int i = 0; i < numThreads; i++)
        {
            int threadNum = i;
            threadIsDone[i] = new ManualResetEvent(true);
            threadCanStart[i] = new ManualResetEvent(false);
            workers[i] = new PoolThread();
        }

    }

    static void Scheduler()
    {
        try
        {

            while (true)
            {
                // Wait for an action

                schedulerEvent.WaitOne();
                //Debug.Log(taskQueue.Count);

                // Wait for free thread
                //int freeThread = WaitHandle.WaitAny(threadIsDone);

                // Add action to thread and start it
                for (int i = 0; i < workers.Length; i++)
                {
                    if (workers[i].isRunning)
                        continue;
                    lock (queueLocker)
                    {
                        for (int j = 0; j < taskQueue.Count || j < 0; j++)
                        {
                            workers[i].GiveTask(taskQueue[j]);
                            taskQueue.RemoveAt(j);
                            break;
                            //Debug.Log(finishedTaskQueue[finishedTaskQueue.Count - 1]);
                            j--;
                        }
                    }
                }


                // If no free actions, deactivate scheduler
                //if (taskQueue.Count == 0)
                //    schedulerEvent.Reset();
            }
        }
        catch (System.Exception e)
        {

            Debug.Log(e);
        }
    }

    public static void AddToFinishedQueue(Task task)
    {

        finishedTaskQueue.Remove(task);
        
    }

}


public class Task
{
    WaitCallback callback;
    object args;
    public bool isRunning = false;
    public int taskNum;

    public Task(WaitCallback callback, object args, int taskNum)
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
    public bool isRunning;
    public Task task;
    Thread thread;
    int threadNum;
    ManualResetEvent threadCanStart;

    public PoolThread()
    {
        threadCanStart = new ManualResetEvent(false);
        thread = new Thread(delegate () { Worker(); });
        thread.IsBackground = true;
        thread.Start();
    }

    public void GiveTask(Task task)
    {
        this.task = task;
        isRunning = true;
        threadCanStart.Set();
    }

    void Worker()
    {
        while (true)
        {
            threadCanStart.WaitOne();
            threadCanStart.Reset();
            task.RunTask();

            isRunning = false;
        }
    }
}

