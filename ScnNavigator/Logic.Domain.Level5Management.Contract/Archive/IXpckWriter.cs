using Logic.Domain.Level5Management.Contract.DataClasses.Archive;
using System.IO;

namespace Logic.Domain.Level5Management.Contract.Archive
{
    public interface IXpckWriter
    {
        void Write(ArchiveData archiveData, Stream output);
    }
}
