namespace SharpBlog.Client.Services
{
    public interface ISettingsService
    {
        string GetBlogName();
        string GetBlogDescription();
        string GetIconRelativeUrl();
        string GetLogoRelativeUrl();

    }
}
