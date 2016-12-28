// MySync © 2016 Damian 'Erdroy' Korczowski

namespace MySync.Client.Core
{
    public delegate void TaskEvent();

    /// <summary>
    /// Task class.
    /// </summary>
    public class Task
    {
        // INTERNAL
        internal int TaskId;

        /// <summary>
        /// Use this for the "job" task which is the main-heavy work to do in separate thread.
        /// WARNING: This is not called by main thread!
        /// </summary>
        public TaskEvent OnJob;

        /// <summary>
        /// This will be dispatched and called in the main thread when the job is done.
        /// </summary>
        public TaskEvent OnDone;

        /// <summary>
        /// True wehn job is done.
        /// </summary>
        public bool IsDone;

        /// <summary>
        /// Is this task started?
        /// </summary>
        public bool IsWorking;
    }
}