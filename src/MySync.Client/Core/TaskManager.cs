// MySync © 2016 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MySync.Client.Core
{
    /// <summary>
    /// Task manager class.
    /// </summary>
    public static class TaskManager
    {
        private static int _lastTaskId = 100;
        private static readonly List<Task> Tasks = new List<Task>();
        private static readonly List<Action> Events = new List<Action>();

        /// <summary>
        /// Queue task.
        /// </summary>
        /// <param name="task"></param>
        public static void QueueTask(Task task)
        {
            var myTask = task;

            TaskCount++;

            lock (Tasks)
            {
                myTask.TaskId = _lastTaskId++;
                Tasks.Add(myTask);
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                TaskProcessor(myTask);
            });
        }

        /// <summary>
        /// Dispatch custom event.
        /// </summary>
        /// <param name="evnt">The event.</param>
        public static void DispathSingle(Action evnt)
        {
            lock (Events)
            {
                Events.Add(evnt);
            }
        }

        // INTERNAL
        internal static void DispatchEvents()
        {
            lock (Tasks)
            {
                var doneTasks = Tasks.Where(task => task.IsDone).ToList();

                // call OnDone and remove for all tasks.
                foreach (var task in doneTasks)
                {
                    task.OnDone();
                    Tasks.Remove(task);
                }
            }

            // call events
            lock (Events)
            {
                foreach (var evnt in Events)
                {
                    evnt();
                }

                Events.Clear();
            }
        }

        // private
        private static void TaskProcessor(Task task)
        {
            // do the task work.
            task.IsWorking = true;
            ActiveTasks++;

            try
            {
                task.OnJob();
            }
            catch
            {
                // ignore
            }

            task.IsDone = true;
            ActiveTasks--;
            TaskCount--;
        }

        /// <summary>
        /// The amout of working tasks.
        /// </summary>
        public static int ActiveTasks { get; private set; }

        /// <summary>
        /// The amout of all tasks(started and queued).
        /// </summary>
        public static int TaskCount { get; private set; }
    }
}