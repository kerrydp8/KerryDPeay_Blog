using System;
using PagedList;
using PagedList.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KerryDPeay_Blog.Helpers;
using KerryDPeay_Blog.Models;
using System.Xml.Linq;

namespace KerryDPeay_Blog.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Admin")] //To access these posts, you must be logged in as an Administrator 
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        [AllowAnonymous]
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 5; // the number of posts you want to display per page             
            int pageNumber = (page ?? 1);

            return View(blogList.ToPagedList(pageNumber, pageSize)); //Lists all of the posts in the order they were created (descending order)
        }

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;

            if (searchStr != null) {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) || 
                p.Body.Contains(searchStr) || p.Comments.Any(c => c.Body.Contains(searchStr) || 
                c.Author.FirstName.Contains(searchStr) || c.Author.LastName.Contains(searchStr) || 
                c.Author.DisplayName.Contains(searchStr) || c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Create);
        }

        public ActionResult AllPosts()
        {
            return View(db.BlogPosts.Where(b => b.Published).ToList());
        }
        public ActionResult AdminIndex()
        {
            var publishedBlogPosts = db.BlogPosts.ToList();
            return View("Index", publishedBlogPosts);

        }

        public ActionResult ModeratorIndex()
        {
            var publishedBlogPosts = db.BlogPosts.ToList();
            return View("Index", publishedBlogPosts);
        }

        /*
        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BlogPost blogPost = db.BlogPosts.Find(id);

            if (blogPost == null)
            {
                return HttpNotFound();
            }

            return View(blogPost);
        }
        */

        [AllowAnonymous]
        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }



        // GET: BlogPosts/Create - This action (Http:Get) is returning a View to the Creation page.
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create - This action (Http:Post) allows the user to create data from the View and send it to the database. 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.MakeSlug(blogPost.Title);

                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }

                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }

                if (String.IsNullOrWhiteSpace(Slug))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaURL = "/Uploads/" + fileName;
                }


                blogPost.Slug = Slug; //Creates a new slug and stores it in a variable
                blogPost.Create = DateTime.Now; //Stores the time in which the post is created. 
                db.BlogPosts.Add(blogPost); //Adds the current post to the collection of posts. 
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

   

        // GET: BlogPosts/Edit/5 - Returns the Edit View only for a particular post. This Action (Http:Get) searches for the post in the 
        //database. 
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            BlogPost blogPost = db.BlogPosts.Find(id);

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var newSlug = StringUtilities.MakeSlug(blogPost.Title);

                if (newSlug != blogPost.Slug) //The slug is used to compare posts. If there is a matching slug in the collection, the user must pick a new title.
                {
                    if (String.IsNullOrWhiteSpace(newSlug))
                    {
                        ModelState.AddModelError("Title", "Invalid Title");
                        return View(blogPost);
                    }

                    if (db.BlogPosts.Any(p => p.Slug == newSlug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);
                    }

                    blogPost.Slug = newSlug;
                    //The new slug, formed from the new title selected by the user becomes the current slug for the post, replacing the former one.

                    if (ImageUploadValidator.IsWebFriendlyImage(image))
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                        blogPost.MediaURL = "/Uploads/" + fileName;
                    }


                }

                blogPost.Update = DateTime.Now;
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5 - Returns the select post from the database for the user to delete (or not).
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BlogPost blogPost = db.BlogPosts.Find(id);

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5 - This action allows the user to delete the post from the database.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
