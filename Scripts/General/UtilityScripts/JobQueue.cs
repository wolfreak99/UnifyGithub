// Original url: http://wiki.unity3d.com/index.php/JobQueue
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/UtilityScripts/JobQueue.cs
// File based on original modification date of: 27 August 2016, at 17:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.UtilityScripts
{
Contents [hide] 
1 Description 
2 Classes 
2.1 JobQueue 
2.2 JobItem 
3 Additional usage notes 
4 Example 
5 JobQueue.cs 

Description This is a simple threaded job queue for any kind jobs you want to execute on a thread. 
The JobQueue class is a management class, the actual job scheduler. When you create an instance of that class you have to specify the "job class" it will manage as generic parameter and how many threads the queue should use. 
The threads are started immediately when the JobQueue is created and put on hold until jobs are queued. 
Classes JobQueue - JobQueue(int aThreadCount) - constructor.
- AddJob(T aJob) - Adds a job item to the queue for processing. If there are unused threads available it will
  immediately schedule the job.
- AddJobFromOtherThreads(T aJob) - This has to be used when you want to schedule new jobs from other threads
  than the main thread.
- int CountActiveJobs() - counts the currently active threads.
- void Update() - Processes the queue on the main thread. Schedules waiting jobs and "finishes" jobs which
  are done.
- void ShutdownQueue() - terminates all active jobs and terminates all running threadd. After using this method
  the the JobQueue can no longer be used.
- event OnJobFinished - An event to which you can subscribe from the outside. It will be invoked each time a job
  finishes. The callback has the JobItem that has finished as parameter. This event is raised on the main
  thread (from inside "Update").
JobItem - bool IsAborted - readonly property to determine if this job should be aborted. The actual work routine should
  check this on a regular basis while executing and terminate itself immediately when this is true
- bool IsDataReady - readonly property to indicate that the job has finished. You may not touch any of the Job
  data while this is not true after you handed the job to the queue.
- abstract void DoWork() - This is the work routine which has to be overriden in a concrete Job class.
- virtual void OnFinished() - This is a callback which will be invoked from the main thread (the thread that
  executed Update on the JobQueue) once the DoWork method has finished.
- void AbortJob() - signals that this job should be aborted. This is called internally when the Jobqueue is
  shutdown and a job is still running.
- void Execute() and void ResetJobState() are used internally by the JobQueue. You should not call these.
Of course the JobItem instances could be pooled if needed. Just keep in mind to only touch an JobItem instance when it's not in use. 
Additional usage notes This implementation avoids locks where possible. It uses a well defined order of instructions in conjunction with state flags to ensure no race conditions when used properly. 
This class is primarily designed for a "single producer, single consumer" layout. So Jobs are only added to the queue from the main thread (the thread that calls Update on the queue). The class supports adding jobs from other threads by using "AddJobFromOtherThreads" instead of "AddJob". This will store the job in an additional "locked" queue which then will be copied into the actual job queue. 
Important: The moment you call AddJob (or AddJobFromOtherThreads), you hand over the job item to the scheduler and you should no longer touch any of the data inside the job item until IsDataReady turns true. Usually you would use one of the callbacks to further process the jobitem on the main thread once it's finished. This can be implemented directly inside the JobItem by overriding "OnFinished", or in a more general manner by subscribing to the "OnJobFinished" event in the JobQueue. Those callbacks will be invoked from inside the Update method. So as long as Update is only called from the main thread those callbacks will always be executed on the main thread. 
Do not call Update from multiple threads!!! One JobQueue should only be managed by a single thread. 


Example JobQueue<CalcLimit2> queue;
 
void OnEnable()
{
    queue = new JobQueue<CalcLimit2>(2); // create new queue with 2 threads
    queue.AddJob(new CalcLimit2 { count = 200, CustomName = "200 iterations" });
    queue.AddJob(new CalcLimit2 { count = 20, CustomName = "Do 20 iterations" });
    queue.AddJob(new CalcLimit2 { count = 9001, CustomName = "over 9000" });
    queue.AddJob(new CalcLimit2 { count = 50000000, CustomName = "50M" });
}
 
void Update()
{
    queue.Update();
}
void OnDisable()
{
    queue.ShutdownQueue(); // important to terminate the threads inside the queue.
}
 
 
// The example job class:
public class CalcLimit2 : JobItem
{
    // some identifying name, not related to the actual job
    public string CustomName;
    // input variables. Should be set before the job is started
    public int count = 5;
    // output / result variable. This represents the data that this job produces.
    public float Result;
    protected override void DoWork()
    {
        // this is executed on a seperate thread
        float v = 0;
        for(int i = 0; i < count; i++)
        {
            v += Mathf.Pow(0.5f, i);
            // check every 100 iteration if the job should be aborted
            if ((i % 100) == 0 && IsAborted)
                return;
        }
        Result = v;
    }
    public override void OnFinished()
    {
        // this is executed on the main thread.
        Debug.Log("Job: " + CustomName + " has finished with the result: " + Result);
    }
}JobQueue.cs /******************************************************************************
 * The MIT License (MIT)
 * 
 * Copyright (c) 2016 Bunny83
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 * 
 * This implements a simple threaded job queue. Simply derive a class from JobItem
 * and override the DoWork method.
 * 
 *****************************************************************************/
 
namespace B83.JobQueue
{
    using System;
    using System.Threading;
    using System.Collections.Generic;
 
    public abstract class JobItem
    {
        private volatile bool m_Abort = false;
        private volatile bool m_Started = false;
        private volatile bool m_DataReady = false;
 
        /// <summary>
        /// This is the actual job routine. override it in a concrete Job class
        /// </summary>
        protected abstract void DoWork();
 
        /// <summary>
        /// This is a callback which will be called from the main thread when
        /// the job has finised. Can be overridden.
        /// </summary>
        public virtual void OnFinished() { }
 
        public bool IsAborted { get { return m_Abort; } }
        public bool IsStarted { get { return m_Started; } }
        public bool IsDataReady { get { return m_DataReady; } }
 
        public void Execute()
        {
            m_Started = true;
            DoWork();
            m_DataReady = true;
        }
 
        public void AbortJob()
        {
            m_Abort = true;
        }
 
        public void ResetJobState()
        {
            m_Started = false;
            m_DataReady = false;
            m_Abort = false;
        }
    }
 
 
    public class JobQueue<T> : IDisposable where T : JobItem
    {
        private class ThreadItem
        {
            private Thread m_Thread;
            private AutoResetEvent m_Event;
            private volatile bool m_Abort = false;
 
            // simple linked list to manage active threaditems
            public ThreadItem NextActive = null;
 
            // the job item this thread is currently processing
            public T Data;
 
            public ThreadItem()
            {
                m_Event = new AutoResetEvent(false);
                m_Thread = new Thread(ThreadMainLoop);
                m_Thread.Start();
            }
 
            private void ThreadMainLoop()
            {
                while (true)
                {
                    if (m_Abort)
                        return;
                    m_Event.WaitOne();
                    if (m_Abort)
                        return;
                    Data.Execute();
                }
            }
 
            public void StartJob(T aJob)
            {
                aJob.ResetJobState();
                Data = aJob;
                // signal the thread to start working.
                m_Event.Set();
            }
 
            public void Abort()
            {
                m_Abort = true;
                if (Data != null)
                    Data.AbortJob();
                // signal the thread so it can finish itself.
                m_Event.Set();
            }
 
            public void Reset()
            {
                Data = null;
            }
        }
        // internal thread pool
        private Stack<ThreadItem> m_Threads = new Stack<ThreadItem>();
        private Queue<T> m_NewJobs = new Queue<T>();
        private volatile bool m_NewJobsAdded = false;
        private Queue<T> m_Jobs = new Queue<T>();
        // start of the linked list of active threads
        private ThreadItem m_Active = null;
 
        public event Action<T> OnJobFinished;
 
        public JobQueue(int aThreadCount)
        {
            if (aThreadCount < 1)
                aThreadCount = 1;
            for (int i = 0; i < aThreadCount; i++)
                m_Threads.Push(new ThreadItem());
        }
 
        public void AddJob(T aJob)
        {
            if (m_Jobs == null)
                throw new System.InvalidOperationException("AddJob not allowed. JobQueue has already been shutdown");
            if (aJob != null)
            {
                m_Jobs.Enqueue(aJob);
                ProcessJobQueue();
            }
        }
 
        public void AddJobFromOtherThreads(T aJob)
        {
            lock (m_NewJobs)
            {
                if (m_Jobs == null)
                    throw new System.InvalidOperationException("AddJob not allowed. JobQueue has already been shutdown");
                m_NewJobs.Enqueue(aJob);
                m_NewJobsAdded = true;
            }
        }
 
        public int CountActiveJobs()
        {
            int count = 0;
            for (var thread = m_Active; thread != null; thread = thread.NextActive)
                count++;
            return count;
        }
 
        private void CheckActiveJobs()
        {
            ThreadItem thread = m_Active;
            ThreadItem last = null;
            while (thread != null)
            {
                ThreadItem next = thread.NextActive;
                T job = thread.Data;
                if (job.IsAborted)
                {
                    if (last == null)
                        m_Active = next;
                    else
                        last.NextActive = next;
                    thread.NextActive = null;
 
                    thread.Reset();
                    m_Threads.Push(thread);
                }
                else if (thread.Data.IsDataReady)
                {
                    job.OnFinished();
                    if (OnJobFinished != null)
                        OnJobFinished(job);
 
                    if (last == null)
                        m_Active = next;
                    else
                        last.NextActive = next;
                    thread.NextActive = null;
 
                    thread.Reset();
                    m_Threads.Push(thread);
                }
                else
                    last = thread;
                thread = next;
            }
        }
 
        private void ProcessJobQueue()
        {
            if(m_NewJobsAdded)
            {
                lock(m_NewJobs)
                {
                    while (m_NewJobs.Count > 0)
                        AddJob(m_NewJobs.Dequeue());
                    m_NewJobsAdded = false;
                }
            }
            while (m_Jobs.Count > 0 && m_Threads.Count > 0)
            {
                var job = m_Jobs.Dequeue();
                if (!job.IsAborted)
                {
                    var thread = m_Threads.Pop();
                    thread.StartJob(job);
                    // add thread to the linked list of active threads
                    thread.NextActive = m_Active;
                    m_Active = thread;
                }
            }
        }
 
        public void Update()
        {
            CheckActiveJobs();
            ProcessJobQueue();
        }
 
        public void ShutdownQueue()
        {
            for (var thread = m_Active; thread != null; thread = thread.NextActive)
                thread.Abort();
            while (m_Threads.Count > 0)
                m_Threads.Pop().Abort();
            while (m_Jobs.Count > 0)
                m_Jobs.Dequeue().AbortJob();
            m_Jobs = null;
            m_Active = null;
            m_Threads = null;
        }
 
        public void Dispose()
        {
            ShutdownQueue();
        }
    }
}
}
