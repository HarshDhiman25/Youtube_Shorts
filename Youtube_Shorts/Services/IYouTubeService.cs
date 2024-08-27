namespace Youtube_Shorts.Services
{
    public interface IYouTubeService
    {
        Task UploadVideo(string filePath, string title, string description, string tags);
    }
}
