using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using System.IO;

namespace Logic.Domain.Level5Management.Contract.Archive
{
    public interface IArchiveReader
    {
        ArchiveData Read(Stream input);
    }
}
