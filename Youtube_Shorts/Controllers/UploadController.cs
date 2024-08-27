using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Youtube_Shorts.Models;
using Youtube_Shorts.Services;

public class UploadController : Controller
{
    private readonly IYouTubeService _youTubeService;

    public UploadController(IYouTubeService youTubeService)
    {
        _youTubeService = youTubeService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadVideo(IFormFile videoFile, string title, string description, string tags)
    {
        if (videoFile != null && videoFile.Length > 0)
        {
            var filePath = Path.Combine(Path.GetTempPath(), videoFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            await _youTubeService.UploadVideo(filePath, title, description, tags);

            // Optionally, delete the temporary file
            System.IO.File.Delete(filePath);

            return RedirectToAction("Index");
        }

        return View("Index");
    }
}

