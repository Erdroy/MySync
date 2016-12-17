// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using MySync.Client.Core.Projects;

namespace MySync.Client.Core
{
    public class ProjectLock : IDisposable
    {
        private readonly Project _proj;

        public ProjectLock(Project proj)
        {
            _proj = proj;
            _proj.Lock();
        }

        public void Dispose()
        {
            _proj.Unlock();
        }
    }
}