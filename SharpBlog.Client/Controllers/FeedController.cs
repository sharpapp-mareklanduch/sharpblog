using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http.Features;
using SharpBlog.Client.Models;
using MoreLinq;
using System;
using Microsoft.SyndicationFeed.Rss;
using Microsoft.SyndicationFeed.Atom;
using Microsoft.SyndicationFeed;
using SharpBlog.Core.Dal;

namespace SharpBlog.Client.Controllers
{
    public class FeedController : Controller
    {
        private readonly IPostDal _postDal;

        public FeedController(IPostDal postDal)
        {
            _postDal = postDal;
        }

        [Route("/robots.txt")]
        public string RobotsTxt()
        {
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *")
                .AppendLine("Disallow:")
                .Append("sitemap: ")
                .Append(this.Request.Scheme)
                .Append("://")
                .Append(this.Request.Host)
                .AppendLine("/sitemap.xml");

            return sb.ToString();
        }

        [Route("/feed/{type?}")]
        public async Task Feed(string type)
        {
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            Response.ContentType = "application/xml";
            var host = $"{Request.Scheme}://{Request.Host}";

            using var xmlWriter = XmlWriter
                                    .Create(Response.Body,
                                    new XmlWriterSettings()
                                    {
                                        Async = true, Indent = true, Encoding = new UTF8Encoding(false)
                                    });

            var posts = await _postDal.GetAll();

            ISyndicationFeedWriter writer;
            if (type?.Equals("rss", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                writer = new RssFeedWriter(xmlWriter);
                await ((RssFeedWriter)writer).WriteValue("link", host).ConfigureAwait(false);
                await ((RssFeedWriter)writer).WriteTitle("title").ConfigureAwait(false);
                await ((RssFeedWriter)writer).WriteDescription("description").ConfigureAwait(false);
                await ((RssFeedWriter)writer).WriteGenerator("SharpBlog").ConfigureAwait(false);
            } 
            else 
            {
                writer = new AtomFeedWriter(xmlWriter);
                await ((AtomFeedWriter)writer).WriteId(host).ConfigureAwait(false);
                await ((AtomFeedWriter)writer).WriteTitle("titles").ConfigureAwait(false);
                await ((AtomFeedWriter)writer).WriteSubtitle("description").ConfigureAwait(false);
                await ((AtomFeedWriter)writer).WriteGenerator("SharpBlog", "https://github.com/sharpapp-mareklanduch/sharpblog", "1.0").ConfigureAwait(false);
                await ((AtomFeedWriter)writer).WriteValue("updated",
                                                            posts
                                                            .Max(p=>p.PublicationDate)
                                                            .Value
                                                            .ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture))
                                                            .ConfigureAwait(false);
            }

            foreach (var post in posts.Where(p => p.IsPublished))
            {
                var item = new AtomEntry
                {
                    Id = host + post.RelativeUrl,
                    Title = post.Title,
                    Description = post.Content,
                    Published = post.PublicationDate.Value,
                    LastUpdated = post.LastModified,
                    ContentType = "html"
                };

                foreach (var category in post.Categories)
                {
                    item.AddCategory(new SyndicationCategory(category.Name));
                }

                item.AddContributor(new SyndicationPerson("test@example.com", "Marek Łańduch"));
                item.AddLink(new SyndicationLink(new Uri(item.Id)));

                await writer.Write(item).ConfigureAwait(false);
            }
        }

        [Route("/sitemap.xml")]
        public async Task Sitemap()
        {
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            var host = $"{Request.Scheme}://{Request.Host}";

            Response.ContentType = "application/xml";

            using var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true });
            xml.WriteStartDocument();
            xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            var posts = await _postDal.GetAll();

            xml.WriteStartElement("url");
            xml.WriteElementString("loc", host);
            xml.WriteElementString("lastmod", posts.Max(p => p.LastModified).ToString("yyyy-MM-ddThh:mmzzz", CultureInfo.InvariantCulture));
            xml.WriteEndElement();

            foreach (var post in posts.Where(x => x.IsPublished))
            {
                xml.WriteStartElement("url");
                xml.WriteElementString("loc", host + post.RelativeUrl);
                xml.WriteElementString("lastmod", post.PublicationDate.Value.ToString("yyyy-MM-ddThh:mmzzz", CultureInfo.InvariantCulture));
                xml.WriteEndElement();
            }

            var categories = posts
                                .Where(x => x.IsPublished)
                                .SelectMany(p => p.Categories.Select(c => c.Name),
                                (p, c) => new CategorySitemap
                                {
                                    CategoryName = c,
                                    LastModified = p.LastModified
                                })
                                .GroupBy(c => c.CategoryName);

            foreach (var category in categories)
            {
                var lastModifiedCategory = category.MaxBy(c => c.LastModified).First();
                xml.WriteStartElement("url");
                xml.WriteElementString("loc", host + lastModifiedCategory.RelativeUrl);
                xml.WriteElementString("lastmod", lastModifiedCategory
                                                    .LastModified
                                                    .ToString("yyyy-MM-ddThh:mmzzz", CultureInfo.InvariantCulture));
                xml.WriteEndElement();
            }

            xml.WriteEndElement();
        }
    }
}
