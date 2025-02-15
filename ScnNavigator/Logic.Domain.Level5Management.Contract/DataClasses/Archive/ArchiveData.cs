using System.Collections.Generic;

namespace Logic.Domain.Level5Management.Contract.DataClasses.Archive
{
    public class ArchiveData
    {
        public byte Type { get; set; }
        public IList<NamedArchiveEntry> Files { get; set; }
    }
}
