using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpBlog.Client.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly Dictionary<string, string> _blogDetails = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _reCaptcha = new Dictionary<string, string>();

        public SettingsService(IConfiguration configuration) {
            configuration.Bind("blogDetails", _blogDetails);
            configuration.Bind("reCaptcha", _reCaptcha);
        }

        public string GetBlogDescription()
        {
            return _blogDetails["description"];
        }

        public string GetBlogName()
        {
            return _blogDetails["name"];
        }

        public string GetIconRelativeUrl()
        {
            return _blogDetails["icon"];
        }

        public string GetLogoRelativeUrl()
        {
            return _blogDetails["logo512x512"];
        }

        public bool GetReCaptchaEnabled()
        {
            bool.TryParse(_reCaptcha["enabled"], out var enabled);
            return enabled;
        }

        public string GetReCaptchaPublicKey()
        {
            return _reCaptcha["publicKey"];
        }

        public string GetReCaptchaPrivateKey()
        {
            return _reCaptcha["privateKey"];
        }
    }
}
