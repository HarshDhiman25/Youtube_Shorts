using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Youtube_Shorts.Models;
using System.Security.Claims;

namespace Youtube_Shorts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claims = User.Claims;

                // Update this line to get the correct claim type for the user's name
                var userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value
                               ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                var userEmail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                // Google returns the profile picture URL in a specific claim type
                var userProfilePicture = claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;

                ViewBag.UserName = userName;
                ViewBag.UserEmail = userEmail;
                ViewBag.UserProfilePicture = userProfilePicture;
            }
            else
            {
                ViewBag.UserName = null;
                ViewBag.UserEmail = null;
                ViewBag.UserProfilePicture = null;
            }

            return View();
        }
    }
}
