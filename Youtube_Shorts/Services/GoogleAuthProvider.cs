using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Youtube_Shorts.Services
{
    public class GoogleAuthProvider 
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleAuthProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<UserCredential> GetUserCredentialsAsync(ClaimsPrincipal user)
        //{
        //    var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        //    if (string.IsNullOrEmpty(accessToken))
        //    {
        //        throw new InvalidOperationException("User is not authenticated or access token is missing.");
        //    }

        //    var credential = GoogleCredential.FromAccessToken(accessToken);
        //    return new UserCredential(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientId = "YOUR_CLIENT_ID",
        //        ClientSecret = "YOUR_CLIENT_SECRET",
        //        Scopes = new[] { "https://www.googleapis.com/auth/youtube.upload" }
        //    }), "user", credential);
        //}
    }

}
