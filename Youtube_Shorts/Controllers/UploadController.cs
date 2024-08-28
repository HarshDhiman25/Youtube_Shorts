using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Youtube_Shorts.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Google;
using Google.Apis.Upload;
using Newtonsoft.Json;


public class UploadController : Controller
{
    //private readonly IGoogleAuthProvider _authProvider;

    public UploadController()
    {
        //_authProvider = authProvider;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadVideo(IFormFile videoFile, string title, string description, string Hashtags)
    {
        if (videoFile == null || videoFile.Length == 0)
        {
            ModelState.AddModelError("", "Please upload a video file.");
            return View("Index");
        }

        // Authenticate user and get access token
        var authenticateResult = await HttpContext.AuthenticateAsync();
        if (!authenticateResult.Succeeded)
        {
            return Challenge(GoogleDefaults.AuthenticationScheme);
        }

        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var expiresIn = await HttpContext.GetTokenAsync("expires_at");

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(expiresIn))
        {
            ModelState.AddModelError("", "Unable to retrieve tokens.");
            return View("Index");
        }

        Console.WriteLine($"Access Token: {accessToken}");
        Console.WriteLine($"Token Expiry Time: {expiresIn}");

        // Check if the token is expired
        if (DateTime.UtcNow >= DateTime.Parse(expiresIn))
        {
            // Refresh the token
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var newTokens = await RefreshAccessTokenAsync(refreshToken);

            if (!newTokens.HasValue)
            {
                ModelState.AddModelError("", "Unable to refresh access token. Please reauthenticate.");
                return Challenge(GoogleDefaults.AuthenticationScheme);
            }

            // Extract values from the tuple
            var (newAccessToken, newExpiresIn) = newTokens.Value;

            accessToken = newAccessToken;
            var extendedExpiresIn = DateTime.UtcNow.AddDays(2).ToString("o"); // Extend the expiration time by 2 days

            // Update the token in the authentication properties
            var authProperties = authenticateResult.Properties;
            authProperties.UpdateTokenValue("access_token", accessToken);
            authProperties.UpdateTokenValue("expires_at", extendedExpiresIn);
            await HttpContext.SignInAsync(authenticateResult.Principal, authProperties);
        }

        var filePath = Path.Combine(Path.GetTempPath(), videoFile.FileName);

        try
        {
            // Save the uploaded file temporarily
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            // Initialize YouTube service with access token
            var credential = GoogleCredential.FromAccessToken(accessToken)
                .CreateScoped("https://www.googleapis.com/auth/youtube.upload");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "AspExternalAuthentication"
            });

            var video = new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = title,
                    Description = description,
                    Tags = Hashtags?.Split(','),
                    CategoryId = "22"
                },
                Status = new VideoStatus
                {
                    PrivacyStatus = "public"
                }
            };

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", stream, "video/*");

                var response = await videosInsertRequest.UploadAsync();

                if (response.Status == UploadStatus.Failed)
                {
                    ModelState.AddModelError("", $"Upload failed: {response.Exception.Message}");
                    return View("Index");
                }
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            return View("Index");
        }
        finally
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        return RedirectToAction("Index");
    }

    private async Task<(string AccessToken, string ExpiresIn)?> RefreshAccessTokenAsync(string refreshToken)
    {
        using (var client = new HttpClient())
        {
            var requestContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", "998799973915-ousa164pt8bgiaicotkbc3jgebrc28lo.apps.googleusercontent.com"),
            new KeyValuePair<string, string>("client_secret", "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"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
        });

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", requestContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                var accessToken = tokenResponse["access_token"];
                var expiresIn = DateTime.UtcNow.AddSeconds(int.Parse(tokenResponse["expires_in"])).ToString("o");

                return (accessToken, expiresIn);
            }
            else
            {
                // Handle error
                return null;
            }
        }
    }


}
