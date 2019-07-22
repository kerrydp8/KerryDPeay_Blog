﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KerryDPeay_Blog.Helpers;
using KerryDPeay_Blog.Models;

namespace KerryDPeay_Blog.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index()
        {
            return View(db.BlogPosts.Where(b => b.Published).ToList());
        }

        [Authorize(Roles ="Admin")]

        public ActionResult AdminIndex()
        {
            var publishedBlogPosts = db.BlogPosts.ToList();
            return View("Index", publishedBlogPosts);

        }

        [Authorize(Roles = "Moderator")]

        public ActionResult ModeratorIndex()
        {
            var publishedBlogPosts = db.BlogPosts.ToList();
            return View("Index", publishedBlogPosts);

        }

        // GET: BlogPosts/Details/5
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

        // GET: BlogPosts/Create - This action (Http:Get) is returning a View to the Creation page, allowing the user to have access to it.
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create - This action (Http:Post) is returning the actual features of this View and allows the user to interact with the View. 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Abstract,Body,Published")] BlogPost blogPost)
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

                blogPost.Slug = Slug; //Creates a new slug and stores it in a variable
                blogPost.Create = DateTimeOffset.Now; //Stores the time in which the post is created. 
                db.BlogPosts.Add(blogPost); //Adds the current post to the collection of posts. 
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5 - Returns the Edit View only for a particular post. This Action (Http:Get) searches for the post by its abstract and slug. 
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

        //public ActionResult Edit([Bind(Include = "Id,Title,Abstract,Slug,Body,MediaURL,Published,Create,Update")] BlogPost blogPost)
        public ActionResult Edit([Bind(Include = "Id,Title,Abstract,Slug,Body,MediaUrl,Published,Created,Updated")] BlogPost blogPost)
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
                    blogPost.Slug = newSlug; //The new slug, formed from the new title selected by the user becomes the current slug for the post, replacing the former one.
                }

                blogPost.Update = DateTimeOffset.Now;
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5 - Returns the View for the user
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

        // POST: BlogPosts/Delete/5 - The actual functionality of the View
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
