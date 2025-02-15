namespace Logic.Domain.Level5Management.Contract.Image
{
    public interface IImageReaderFactory
    {
        IImageReader Create(int version);
    }
}
