using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KerryDPeay_Blog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Abstract { get; set; }

        public string Slug { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public string MediaURL { get; set; }

        public bool Published { get; set; }

        public DateTimeOffset Create { get; set; }

        public DateTimeOffset? Update { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public BlogPost()
        {
            Comments = new HashSet<Comment>();
        }

    }
}