using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SharpBlog.Client.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly Dictionary<string, string> _blogDetails = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _reCaptcha = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _user = new Dictionary<string, string>();
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public SettingsService(
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            configuration.Bind("blogDetails", _blogDetails);
            configuration.Bind("reCaptcha", _reCaptcha);
            configuration.Bind("user", _user);
            _configuration = configuration;
            _environment = environment;
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

        public bool GetResetUser()
        {
            bool.TryParse(_user["reset"], out var resetUser);
            return resetUser;
        }

        public void SetUserRegistered()
        {
            UpdateOptions(config =>
            {
                config["user"]["reset"] = false;
            });
        }

        private void UpdateOptions(Action<JObject> callback)
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var fi = fileProvider.GetFileInfo("appsettings.json");
            var config = JObject.Parse(File.ReadAllText(fi.PhysicalPath));

            callback(config);

            File.WriteAllText(fi.PhysicalPath, config.ToString());
        }
    }
}
