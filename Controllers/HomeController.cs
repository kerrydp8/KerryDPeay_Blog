using KerryDPeay_Blog.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KerryDPeay_Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 3; // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);

            var publishedPosts = db.BlogPosts.Where(b => b.Published).OrderByDescending(b => b.Create);
            return View(publishedPosts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var from = model.FromName + "," + $"{model.FromEmail}<{ConfigurationManager.AppSettings["emailto"]}>"; //THe name and address of the person who entered it.

                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject, //The subject of the email.
                        Body = model.Body, //The body of the email.
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return View(new EmailModel());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
            //return View("Index", "BlogPosts"); - Does not work
        }
    }
}