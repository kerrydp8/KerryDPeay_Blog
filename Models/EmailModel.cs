using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KerryDPeay_Blog.Models
{
    public class EmailModel
    {
        [Required, Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} character long.", MinimumLength = 1)]
        public string FromName { get; set; }
        [Required, Display(Name = "Email"), EmailAddress]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} character long.", MinimumLength = 1)]
        public string FromEmail { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} character long.", MinimumLength = 1)]
        public string Subject { get; set; }
        [Required]
        [AllowHtml] //THIS IS NEEDED. Html must be allowed to use a RTE.
        public string Body { get; set; }
    }
}