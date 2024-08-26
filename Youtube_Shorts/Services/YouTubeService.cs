using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.IO;
using System.Threading.Tasks;

public class YouTubeService
{
    private readonly YouTubeService _youtubeService;

    public YouTubeService(string apiKey)
    {
        _youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApplicationName = "YourAppName",
            ApiKey = apiKey
        });
    }

    public async Task UploadVideoAsync(string filePath, string title, string description, string[] tags)
    {
        var video = new Video();

        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            var videoInsertRequest = _youtubeService.Videos.Insert(new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = title,
                    Description = description,
                    Tags = tags
                },
                Status = new VideoStatus
                {
                    PrivacyStatus = "public" // or "private"
                }
            }, "video/*", fileStream, "video/*");

            await videoInsertRequest.UploadAsync();
        }
    }
}
