using System;

namespace SharpBlog.Client.Models
{
    public class CategorySitemap
    {
        public string CategoryName { get; set; }
        public DateTime LastModified { get; set; }
        public string RelativeUrl
        {
            get
            {
                return $"/Category/{CategoryName}";
            }
        }
    }
}
