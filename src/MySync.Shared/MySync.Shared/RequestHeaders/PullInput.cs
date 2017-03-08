// MySync © 2016-2017 Damian 'Erdroy' Korczowski

namespace MySync.Shared.RequestHeaders
{
    public class PullInput
    {
        public ProjectAuthority Authority { get; set; }

        public int CommitId;
    }
}
