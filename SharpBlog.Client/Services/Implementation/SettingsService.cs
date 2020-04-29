using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpBlog.Client.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

        public SettingsService(IConfiguration configuration) {
            configuration.Bind("blogDetails", _settings);
        }

        public string GetBlogDescription()
        {
            return _settings["description"];
        }

        public string GetBlogName()
        {
            return _settings["name"];
        }

        public string GetIconRelativeUrl()
        {
            return _settings["icon"];
        }

        public string GetLogoRelativeUrl()
        {
            return _settings["logo512x512"];
        }
    }
}
