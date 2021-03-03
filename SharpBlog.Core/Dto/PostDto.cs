using System;
using System.Collections.Generic;

namespace SharpBlog.Core.Dto
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public bool IsPublished { get; set; }

		public DateTime? PublicationDate { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime InputDate { get; set; }

        public IEnumerable<CategoryDto> Categories { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }

        public string RelativeUrl
        {
            get
            {
                return $"/Blog/Post/{Id}";
            }
        }
    }
}