using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Youtube_Shorts.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

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

                // Get the user's name
                var userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value
                               ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                               ?? claims.FirstOrDefault(c => c.Type == "name")?.Value; // Added fallback for 'name'

                // Get the user's email
                var userEmail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var pfPic = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

                // Get the user's profile picture URL
                var userProfilePicture = claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value
                                         ?? claims.FirstOrDefault(c => c.Type == "picture")?.Value; // Added fallback for 'picture'

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


        [Route("signin-google")]
        public IActionResult SignInGoogle()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, GoogleDefaults.AuthenticationScheme);
        }
    }
}
