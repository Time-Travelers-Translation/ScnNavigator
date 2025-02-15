namespace Logic.Domain.Level5Management.Contract.Image
{
    public interface IImageWriterFactory
    {
        IImageWriter Create(int version);
    }
}
