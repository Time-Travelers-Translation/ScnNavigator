namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IFontReaderFactory
    {
        IFontReader Create(int version);
    }
}
