namespace Threax.DockerTools.Services
{
    public interface IImageManager
    {
        string FindLatestImage(string image, string baseTag, string currentTag);
    }
}