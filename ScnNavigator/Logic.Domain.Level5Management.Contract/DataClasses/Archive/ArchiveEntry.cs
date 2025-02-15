using System.IO;

namespace Logic.Domain.Level5Management.Contract.DataClasses.Archive
{
    public abstract class ArchiveEntry
    {
        public Stream Content { get; set; }
    }
}
