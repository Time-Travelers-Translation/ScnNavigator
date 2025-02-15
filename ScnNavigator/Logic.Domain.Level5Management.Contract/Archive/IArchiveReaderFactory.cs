using Logic.Domain.Level5Management.Contract.Enums.Archive;

namespace Logic.Domain.Level5Management.Contract.Archive
{
    public interface IArchiveReaderFactory
    {
        IArchiveReader Create(ArchiveType type);
    }
}
