using Logic.Domain.Level5Management.Contract.DataClasses.Image;

namespace Logic.Domain.Level5Management.Contract.Image
{
    public interface IImageEncoder
    {
        RawImageData Encode(ImageData image);
    }
}
