//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
//using Google.Apis.YouTube.v3;
//using Google.Apis.YouTube.v3.Data;
//using Microsoft.AspNetCore.Authentication;
//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Youtube_Shorts.Services;

//public class YouTubeUploadService : IYouTubeService
//{
//    private readonly IAuthenticationService _authenticationService;

//    public YouTubeUploadService(IAuthenticationService authenticationService)
//    {
//        _authenticationService = authenticationService;
//    }

//    public async Task UploadVideo(string filePath, string title, string description, string tags)
//    {
//        UserCredential credential;

//        // Load the client secrets from the file
//        using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
//        {
//            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
//                GoogleClientSecrets.FromStream(stream).Secrets,
//                new[] { YouTubeService.Scope.YoutubeUpload },
//                "user",
//                CancellationToken.None,
//                new FileDataStore("YouTube.Auth.Store")
//            );
//        }

//        // Create the YouTube service instance
//        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
//        {
//            HttpClientInitializer = credential,
//            ApplicationName = "Youtube_Shorts"
//        });

//        // Create a new video object
//        var video = new Video
//        {
//            Snippet = new VideoSnippet
//            {
//                Title = title,
//                Description = description,
//                Tags = tags.Split(','),
//                CategoryId = "22" // Category ID for 'People & Blogs'
//            },
//            Status = new VideoStatus
//            {
//                PrivacyStatus = "public" // Set to "private" or "unlisted" if needed
//            }
//        };

//        // Upload the video file
//        using var fileStream = new FileStream(filePath, FileMode.Open);

//        var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
//        videosInsertRequest.ProgressChanged += progress => Console.WriteLine(progress.Status);
//        videosInsertRequest.ResponseReceived += response => Console.WriteLine($"Video uploaded. ID: {response.Id}");

//        // Execute the upload
//        await videosInsertRequest.UploadAsync();
//    }
//}
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Youtube_Shorts.Services;

public class YouTubeUploadService : IYouTubeService
{
    public async Task UploadVideo(string filePath, string title, string description, string tags)
    {
        UserCredential credential;

        try
        {
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("YouTube.Auth.Store")
                );

                if (credential.Token.IsExpired(credential.Flow.Clock))
                {
                    await credential.RefreshTokenAsync(CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to authenticate or authorize YouTube API.", ex);
        }

        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
        });

        var video = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = title,
                Description = description,
                Tags = tags.Split(','),
                CategoryId = "22"
            },
            Status = new VideoStatus
            {
                PrivacyStatus = "public"
            }
        };

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

        videosInsertRequest.ProgressChanged += progress =>
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine($"{progress.BytesSent} bytes sent.");
                    break;
                case UploadStatus.Failed:
                    Console.WriteLine($"An error prevented the upload from completing: {progress.Exception}");
                    break;
            }
        };

        videosInsertRequest.ResponseReceived += response =>
        {
            Console.WriteLine($"Video id '{response.Id}' was successfully uploaded.");
        };

        await videosInsertRequest.UploadAsync();
    }
}
