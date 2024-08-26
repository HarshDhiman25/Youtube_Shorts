using Microsoft.AspNetCore.Http;

public class UploadVideoViewModel
{
    public IFormFile VideoFile { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Hashtags { get; set; }
}
