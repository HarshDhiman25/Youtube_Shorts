using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

public class UploadController : Controller
{
    private readonly YouTubeService _youTubeService;

    public UploadController(YouTubeService youTubeService)
    {
        _youTubeService = youTubeService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile videoFile, string title, string description, string hashtags)
    {
        if (videoFile != null && videoFile.Length > 0)
        {
            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            var tags = hashtags.Split(',').Select(tag => tag.Trim()).ToArray();
            await _youTubeService.UploadVideoAsync(filePath, title, description, tags);

            System.IO.File.Delete(filePath);
            return RedirectToAction("Index");
        }

        return View("Error", new ErrorViewModel { RequestId = "Failed to upload video." });
    }
}
