using Google.Apis.Auth.OAuth2;
using System.Security.Claims;

namespace Youtube_Shorts.Services
{
    public interface IGoogleAuthProvider
    {
        Task<string> GetAccessTokenAsync();
    }

}
