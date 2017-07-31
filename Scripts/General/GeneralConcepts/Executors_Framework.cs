// Original url: http://wiki.unity3d.com/index.php/Executors_Framework
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/GeneralConcepts/Executors_Framework.cs
// File based on original modification date of: 26 March 2012, at 16:08. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
Author: Magnus Wolffelt 
Contents [hide] 
1 Description 
2 Usage 
3 Why? 
3.1 What's wrong with Begin/EndInvoke? 
3.2 Why ICallable interface, and not simply delegates? 
4 Code 
4.1 IExecutor.cs 
4.2 ICallable.cs 
4.3 Future.cs 
4.4 ImmediateExecutor.cs 
4.5 SingleThreadExecutor.cs 
4.6 WorkItem.cs 
4.7 ExecutionException.cs 
4.8 ExecutorTester.cs 
4.9 ExecutionManager.cs 
4.10 Example.cs 

Description This is a small framework designed to assist in the usage of multiple threads in a C#/.Net program. Multi-threading is a very complex subject, and it's really hard to write bug-free multi-threaded code - hence the need for frameworks that make it a little easier. 
The way this works is with the concepts of "Executors", "Callables" and "Futures". An executor is an object that consumes Callable objects (work tasks), and returns Future objects. These Future objects have a generic parameter which is the result type of the computation. The Future objects can also be polled for completion, or they can be requsted to return the result - which blocks the calling thread until the result has been computed. 
While similar to the AsyncOperation provided by Unity, this concept is primarily inspired by the Java standard library, which features an extensive collection of implementations for this purpose. 
Usage To execute tasks, you need an executor: 
    IExecutor myExecutor = new ImmediateExecutor(); // or new SingleThreadExecutor() for exampleThen you submit tasks: 
    Future<int> myFuture1 = myExecutor.Submit(new MultiplyIntsTask(5, 7));
    Future<int> myFuture2 = myExecutor.Submit(new MultiplyIntsTask(5, 12));And then you can either poll: 
    if(myFuture1.IsDone) { int myResult = myFuture1.GetResult(); }or... get the result directly (blocking call): 
    int myResult = myFuture1.GetResult(); // Blocks until result is readyNote that any exception cast during the computation task, will be thrown when calling GetResult(). 
Also, the ExecutionManager helper component can take care of the polling, and do a delegate callback from the Unity thread, which is safe. 

The MultiplyIntsTask looks like this: 
    class MultiplyIntsTask : ICallable<int> {
      int a;
      int b;
      public MultiplyIntsTask(int a, int b) {
        this.a = a;
        this.b = b;
      }
      public int Call() {
        return a * b;
      }
    }This particular task is very fast and really not a good candidate for threaded processing. Consider this an illustrative example only. 
Why? (Skip this section if you're already convinced! ;)) 
The primary advantage over AsyncOperation and .Net Begin/EndInvoke is that tasks in this framework are executed by an explicitly specified executor, which means one can easily change the manner in which async tasks are executed. For example, simply exchange the SingleThreadExecutor instantiation for an ImmediateExecutor, and you are no longer using multi-threading at all, but your other code remains exactly the same as before. 
If someone implemented a thread-pool executor with multiple threads processing submitted tasks, the interface would remain the same, and you could easily switch from no threading, to dual-threading, to pooled threading, with no modifications to existing code. This is really convenient in some cases where you are not sure of the best way, or you want it to be configurable in runtime. 
What's wrong with Begin/EndInvoke? Like stated above, Begin/EndInvoke does not provide a means for controlling the way tasks are executed. It accesses a global (VM scope even?) thread pool, to which task are submitted. So you can't easily switch between threaded and non-threaded approaches - not even in compile time. Executors Framework lets the user change execution style even in runtime, by just replacing the executor object. This may sound like a small detail, but for me it is important and has been very useful on several occasions. 
Why ICallable interface, and not simply delegates? This is a good question, and the framework might switch to delegates in the future. However, some concerns surfaced when delegates were tried briefly: 
Anonymous delegates can behave "oddly"[1] when using outer variables, and invoked later. For example, it's not safe to use an iterator int value in an anonymous delegate object, unless the delegate is invoke immediately before the iterator variable is incremented. 
Sometimes you will want to, later, access and inspect the data passed to the execution task that finished, which is trivial if using classes implementing the ICallable interface, but more complex when using delegate objects. 
Until I feel that these issues have been resolved in a satisfying way, the framework will stick to the ICallable interface, which makes it more apparent what is going on with variables and data. 
Code There are 10 files: 
IExecutor.cs 
ICallable.cs 
Future.cs 
ImmediateExecutor.cs 
SingleThreadExecutor.cs 
WorkItem.cs 
ExecutionException.cs 
ExecutorTester.cs (Unit tests, not required for usage) 
ExecutionManager.cs (Unity helper component, not required for usage) 
Example.cs (Helper component usage example, not required for usage) 
Download all as one zipped unity package File:Executors Framework.zip 
IExecutor.cs using System;
 
namespace Executors {
 
	/// <summary>
	/// Common interface for all executors that can execute tasks.
	/// Tasks are also known as ICallable objects.
	/// </summary>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public interface IExecutor {
		Future<T> Submit<T>(ICallable<T> callable);
		bool IsShutdown();
		void Shutdown();
		int GetQueueSize();
	}
 
 
	/// <summary>
	/// Optional shutdown mode specified when creating certain
	/// types of executors. Note that this is not applicable
	/// to immediate executors.
	/// Default is FinishAll.
	/// </summary>
	public enum ShutdownMode {
		FinishAll,
		CancelQueuedTasks
	}
}ICallable.cs using System;
 
namespace Executors {
 
	/// <summary>
	/// Callable object that returns type T, and may throw an exception.
	/// WARNING: Do not make Unity calls from a potentially threaded work task.
	/// Unity is generally not thread-safe.
	/// </summary>
	/// <typeparam name="T">Type of the computation result object</typeparam>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public interface ICallable<T> {
		T Call();
	}
}Future.cs using System;
using System.Collections.Generic;
using System.Threading;
 
namespace Executors {
 
 
	public interface IFuture {
		bool IsDone { get; }
	}
 
 
	/// <summary>
	/// A Future represents the result of a potentially asynchronous computation.
	/// Methods/properties are available to check if the operation is done or not.
	/// If an execption is thrown during the computation, this exception will be thrown
	/// when calling GetResult().
	/// </summary>
	/// <typeparam name="T">Type of the computation result object</typeparam>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public class Future<T> : IFuture {
 
		private T result;
		private Exception exception = null;
 
		volatile bool isDone = false;
		/// <summary>
		/// Is the computation done?
		/// </summary>
		public bool IsDone {
			get { return isDone; }
		}
 
 
		internal void SetResult(T result) {
			this.result = result;
		}
 
		internal void SetException(Exception e) {
			exception = e;
		}
 
		internal void SetDone() {
			isDone = true;
		}
 
 
		/// <summary>
		/// Get the result of the computation.
		/// Blocks until the computation is done.
		/// </summary>
		public T GetResult() {
			// Could maybe do this with monitor instead.
			while(!IsDone) {
				Thread.Sleep(1);
			}
 
			if(exception != null) {
				throw exception;
			}
 
			return result;
		}
	}
}ImmediateExecutor.cs using System;
 
namespace Executors {
 
	/// <summary>
	/// Non-threaded immediate executor.
	/// Mainly a convenience executor - makes it easy
	/// to switch between threaded and non-threaded approaches.
	/// </summary>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public class ImmediateExecutor : IExecutor {
		private bool shutdown = false;
 
		#region IExecutor Members
 
		public Future<T> Submit<T>(ICallable<T> callable) {
			if(shutdown) {
				throw new InvalidOperationException("May not submit tasks after shutting down executor.");
			}
			Future<T> future = new Future<T>();
			WorkItem<T> task = new WorkItem<T>(callable, future);
			((IWorkItem)task).Execute();
			return future;
		}
 
		public bool IsShutdown() {
			return shutdown;
		}
 
		public void Shutdown() {
			shutdown = true;
		}
 
		public int GetQueueSize() {
			return 0;
		}
 
		#endregion
 
 
	}
}SingleThreadExecutor.cs using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
 
namespace Executors {
 
	/// <summary>
	/// Single threaded executor. Useful for asynchronous operations
	/// without making the program overly complex.
	/// </summary>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	class SingleThreadExecutor : IExecutor {
		private Thread workerThread = null;
		private readonly Queue<IWorkItem> taskQueue = new Queue<IWorkItem>();
		private readonly object locker = new object();
 
		private ShutdownMode shutdownMode;
		volatile bool shutdown = false;
		volatile bool shutdownCompleted = false;
 
 
		public SingleThreadExecutor() : this(ShutdownMode.FinishAll) { }
 
		public SingleThreadExecutor(ShutdownMode shutdownMode) {
			this.shutdownMode = shutdownMode;
			ThreadStart start = new ThreadStart(RunWorker);
			workerThread = new Thread(start);
			workerThread.Start();
		}
 
 
		void RunWorker() {
			while(!shutdown) {
				lock(locker) {
					while(taskQueue.Count == 0 && !shutdown) {
						Monitor.Wait(locker);
					}
				}
 
				while(taskQueue.Count > 0) {
					bool shouldCancel = (shutdown && shutdownMode.Equals(ShutdownMode.CancelQueuedTasks));
					if(shouldCancel) {
						break;
					}
 
					IWorkItem task = null;
					lock(locker) {
						if(taskQueue.Count > 0) {
							task = taskQueue.Dequeue();
						}
					}
					if(task != null) {
						task.Execute();
					}
				}
			}
 
			foreach(IWorkItem task in taskQueue) {
				task.Cancel("Shutdown");
			}
 
			shutdownCompleted = true;
		}
 
 
		#region IExecutor Members
 
		public Future<T> Submit<T>(ICallable<T> callable) {
			lock(locker) {
				if(shutdown) {
					throw new InvalidOperationException("May not submit tasks after shutting down executor.");
				}
				Future<T> future = new Future<T>();
				WorkItem<T> task = new WorkItem<T>(callable, future);
				taskQueue.Enqueue(task);
				Monitor.Pulse(locker);
				return future;
			}
		}
 
		public bool IsShutdown() {
			return shutdownCompleted;
		}
 
		public void Shutdown() {
			lock(locker) {
				shutdown = true;
				Monitor.Pulse(locker);
			}
		}
 
		public int GetQueueSize() {
			// FIXME: Find out if lock is really necessary here.
			lock(locker) {
				return taskQueue.Count;
			}
		}
 
		#endregion
	}
}WorkItem.cs using System;
 
namespace Executors {
 
	/// <summary>
	/// Internal type used by executors to associate Future objects with
	/// callables, and to call the callable and set appropriate fields
	/// in the Future object.
	/// The non-generic interface is needed for the Executor code.
	/// </summary>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	internal interface IWorkItem {
		void Execute();
		void Cancel(string reason);
	}
 
	internal class WorkItem<T> : IWorkItem {
		public readonly ICallable<T> callable;
		public readonly Future<T> future;
 
		public WorkItem(ICallable<T> callable, Future<T> future) {
			this.callable = callable;
			this.future = future;
		}
 
		public void Execute() {
			try {
				T result = callable.Call();
				future.SetResult(result);
			} catch(Exception e) {
				future.SetException(new ExecutionException(e));
			} finally {
				future.SetDone();
			}
		}
 
		public void Cancel(string reason) {
			if(future.IsDone) {
				throw new InvalidOperationException("Can not cancel a future that is done.");
			}
			future.SetException(new ExecutionException(new Exception("Task was cancelled due to: " + reason)));
			future.SetDone();
		}
	}
}ExecutionException.cs using System;
 
namespace Executors {
 
	/// <summary>
	/// Wrapper exception type for exceptions thrown during
	/// execution of an ICallable.
	/// </summary>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public class ExecutionException : Exception {
 
		public readonly Exception delayedException;
 
		public ExecutionException(Exception delayedException) {
			this.delayedException = delayedException;
		}
	}
}ExecutorTester.cs using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
 
namespace Executors {
 
	/// <summary>
	/// Simple class for (basic) unit testing of executors.
	/// </summary>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public class ExecutorTester {
 
		class MultiplyTask : ICallable<double> {
		    int a;
		    int b;
		    public MultiplyTask(int a, int b) {
		        this.a = a;
		        this.b = b;
		    }
 
		    public double Call() {
				Thread.Sleep(10);
		        return (double)a * b;
		    }
		}
 
 
		class ExceptionThrowingTask : ICallable<int> {
		    public int Call() {
		        throw new ExecutionException(new Exception("Task thrown exception."));
		    }
		}
 
		bool doLogging;
 
 
		public ExecutorTester(bool doLogging)
		{
			this.doLogging = doLogging;
		}
 
		private void Log(string msg)
		{
			if (doLogging)
			{
				Debug.Log("ExecutorTester: " + msg);
			}
		}
 
 
		public void TestAllExecutors() {
			List<IExecutor> toBeTested = new List<IExecutor>();
			toBeTested.Add(new ImmediateExecutor());
			toBeTested.Add(new SingleThreadExecutor());
 
			foreach(IExecutor executor in toBeTested) {
				DoBasicTest(executor);
			}
 
			foreach(IExecutor executor in toBeTested) {
				DoExceptionTest(executor);
			}
 
			foreach(IExecutor executor in toBeTested) {
				DoShutdownTest(executor);
			}
 
			DoShutdownWithPendingTasksTest(new SingleThreadExecutor());
			DoShutdownWithPendingTasksTest(new SingleThreadExecutor(ShutdownMode.CancelQueuedTasks));
		}
 
 
 
 
 
		void DoBasicTest(IExecutor executor) {
 
			List<Future<double>> futures = new List<Future<double>>();
			List<double> expectedAnswers = new List<double>();
 
			for(int i = 1; i < 10; i++) {
				futures.Add(executor.Submit(new MultiplyTask(i, i)));
				//futures.Add(executor.Submit<double>(delegate () { return i*i; } ));
				expectedAnswers.Add(i * i);
			}
 
			for(int i = 0; i < futures.Count; i++) {
				AssertAlmostEqual(futures[i].GetResult(), expectedAnswers[i]);
				AssertTrue(futures[i].IsDone);
 
				Log("Basic test " + i + " with executor type " + executor.GetType().Name + " passed.");
			}
		}
 
 
 
		void DoExceptionTest(IExecutor executor) {
			List<Future<int>> futures = new List<Future<int>>();
 
			for(int i = 0; i < 10; i++) {
				futures.Add(executor.Submit(new ExceptionThrowingTask()));
				//futures.Add(executor.Submit<int>(
				//	delegate () { throw new ExecutionException(new Exception("Task thrown exception.")); }));
			}
 
			for(int i = 0; i < futures.Count; i++) {
				try {
					futures[i].GetResult();
					// Not good
					throw new Exception("Shouldn't be here...");
				} catch(ExecutionException) {
					// All good
				}
 
				AssertTrue(futures[i].IsDone);
 
				Log("Exception test " + i + " with executor type " + executor.GetType().Name + " passed.");
			}
		}
 
		void DoShutdownTest(IExecutor executor) {
			executor.Shutdown();
 
			for(int i = 0; i < 20; i++) {
				if(executor.IsShutdown()) {
					Log("Shutdown test with executor type " + executor.GetType().Name + " passed.");
					return;
				} else {
					Thread.Sleep(100);
				}
			}
			throw new Exception("Executor " + executor.GetType().Name + " failed to shutdown in a timely manner.");
		}
 
 
		void DoShutdownWithPendingTasksTest(IExecutor executor) {
 
			List<Future<double>> futures = new List<Future<double>>();
			for(int i = 0; i < 20; i++) {
				futures.Add(executor.Submit(new MultiplyTask(i, i)));
			}
 
			Thread.Sleep(100);
			DoShutdownTest(executor);
			int queueSize = executor.GetQueueSize();
			Log("Items in queue after shutdown: " + queueSize);
			int successCount = 0;
			int cancelledCount = 0;
			for(int i = 0; i < futures.Count; i++) {
				try {
					if(!futures[i].IsDone) {
						throw new Exception("All queued tasks should have been set to done during shutdown.");
					}
					double result = futures[i].GetResult();
					successCount++;
				} catch(ExecutionException) {
					cancelledCount++;
				}
			}
			AssertEqual(queueSize, cancelledCount);
			Log("Shutdown with pending tasks: " + successCount + " completed, " + cancelledCount + " cancelled.");
		}
 
 
		void AssertTrue(bool condition) {
			if(!condition) {
				throw new Exception("Condition not true.");
			}
		}
 
		void AssertEqual(int i1, int i2) {
			if(i1 != i2) {
				throw new Exception("Numbers are not equal: " + i1 + " , " + i2);
			}
		}
 
		void AssertAlmostEqual(double d1, double d2) {
			if(System.Math.Abs(d1 - d2) > 0.0000001f) {
				throw new Exception("Numbers are not equal: " + d1 + " , " + d2);
			}
		}
 
	}
}ExecutionManager.cs using System;
using System.Collections.Generic;
using UnityEngine;
using Executors;
using System.Threading;
 
 
/// <summary>
/// Unity helper component for convenient usage of the Executors Framework.
/// </summary>
/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
[AddComponentMenu("Executors Framework/Execution Manager")]
public class ExecutionManager : MonoBehaviour {
	public delegate void TaskFinishedHandler<T>(ICallable<T> finishedTask, Future<T> finishedFuture);
 
	private interface IManagedTask {
		void CallCallback();
		bool IsDone { get; }
	}
 
	private class ManagedTask<T> : IManagedTask {
		ICallable<T> callable;
		Future<T> future;
		TaskFinishedHandler<T> finishedHandler;
 
		public ManagedTask(ICallable<T> callable, Future<T> future, TaskFinishedHandler<T> finishedHandler) {
			this.callable = callable;
			this.future = future;
			this.finishedHandler = finishedHandler;
		}
 
		public bool IsDone { get { return future.IsDone; } }
 
		public void CallCallback() {
			finishedHandler(callable, future);
		}
	}
 
 
	/// <summary>
	/// The number of worker threads to use for execution.
	/// Can currently be 0 (immediate) or 1 (single worker thread).
	/// </summary>
	public int threadCount = 0;
 
	/// <summary>
	/// The number of queued tasks, at which the execution manager
	/// will log warning messages.
	/// </summary>
	public int taskCountWarningThreshold = 100;
	private IExecutor executor;
 
	private List<IManagedTask> managedTasks = new List<IManagedTask>();
 
 
	void Awake() {
 
		new ExecutorTester(true).TestAllExecutors();
 
		if(threadCount == 0) {
			executor = new ImmediateExecutor();
		} else if(threadCount == 1) {
			executor = new SingleThreadExecutor();
		} else {
			throw new NotImplementedException("Currently only 0-1 thread executors are supported.");
		}
	}
 
 
	void FixedUpdate() {
 
		foreach(IManagedTask managedTask in managedTasks) {
			if(managedTask.IsDone) {
				managedTask.CallCallback();
			}
		}
 
		managedTasks.RemoveAll(delegate(IManagedTask managedTask) { return managedTask.IsDone; });
	}
 
 
	void OnApplicationQuit() {
		executor.Shutdown();
		while(!executor.IsShutdown()) {
			Thread.Sleep(10);
		}
	}
 
 
	/// <summary>
	/// Submits a task for execution, and calls provided delegate
	/// when the task has been completed.
	/// </summary>
	/// <typeparam name="T">Type of task computation result</typeparam>
	/// <param name="task">Task to execute</param>
	/// <param name="finishedHandler">Handler to call when task has been completed.
	/// Can be null.</param>
	public void SubmitAndManage<T>(ICallable<T> task, TaskFinishedHandler<T> finishedHandler) {
		if(managedTasks.Count >= taskCountWarningThreshold) {
			Debug.LogWarning("Execution Manager on " + gameObject.name + " currently has " + managedTasks.Count + " work tasks in queue.");
		}
 
		Future<T> future = executor.Submit<T>(task);
		managedTasks.Add(new ManagedTask<T>(task, future, finishedHandler));
	}
 
 
	/// <summary>
	/// Submits a task for execution, but does not store the future object.
	/// Will not call back in any way when the task is done - user is expected
	/// to poll the Future object.
	/// </summary>
	/// <typeparam name="T">Type of task computation result</typeparam>
	/// <param name="task">Task to execute</param>
	/// <returns>A future object that can be polled for completion</returns>
	public Future<T> SubmitAndForget<T>(ICallable<T> task) {
		return executor.Submit<T>(task);
	}
 
 
	/// <summary>
	/// Gets the number of items queued up for execution.
	/// </summary>
	public int GetQueueSize() {
		return executor.GetQueueSize();
	}
}Example.cs using System;
using UnityEngine;
using Executors;
using System.Threading;
 
/// <summary>
/// Simple demonstration of how to use an ExecutionManager.
/// </summary>
/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
[RequireComponent(typeof(ExecutionManager))]
[AddComponentMenu("Executors Framework/Example")]
public class Example : MonoBehaviour {
 
	private class LenghtyTask : ICallable<int> {
		public int a;
		public int creationFrameNumber;
 
		public LenghtyTask(int a, int creationFrameNumber) {
			this.a = a;
			this.creationFrameNumber = creationFrameNumber;
		}
 
		public int Call() {
			Thread.Sleep(20);
			return a * a;
		}
	}
 
 
 
	void LenghtyTaskFinishedHandler(ICallable<int> task, Future<int> result) {
		LenghtyTask ourTask = task as LenghtyTask;
		Debug.Log("Frame #"+ Time.frameCount + ": Task with a=" + ourTask.a + 
			" created on frame #" + ourTask.creationFrameNumber + 
			" resulted in: " + result.GetResult());
	}
 
 
	void FixedUpdate() {
		ExecutionManager manager = GetComponent<ExecutionManager>();
		if(manager.GetQueueSize() < 5) {
			ICallable<int> task = new LenghtyTask(UnityEngine.Random.Range(5, 20), Time.frameCount);
			manager.SubmitAndManage(task, LenghtyTaskFinishedHandler);
		}
 
	}
}
}
