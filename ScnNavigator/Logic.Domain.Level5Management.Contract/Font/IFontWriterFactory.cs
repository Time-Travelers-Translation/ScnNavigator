namespace Logic.Domain.Level5Management.Contract.Font
{
    public interface IFontWriterFactory
    {
        IFontWriter Create(int version);
    }
}
