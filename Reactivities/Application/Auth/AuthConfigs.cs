using System;

namespace Application.Auth
{
    public static class AuthConfigs
    {
        public static readonly string AuthCookieName = "autwjt6712";
        public static readonly DateTime TokenExpiryDate = DateTime.UtcNow.AddDays(3);
            
    }
}