﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookCollection.DAL;
using BookCollection.Models;
using PagedList;

namespace BookCollection.Controllers
{
    public class AuthorsController : BaseController
    {
        public AuthorsController(IBookRepository rep, IBookContext bc) : base(rep, bc) { /* constructor forward to BaseController */ }

        // GET: Authors
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, bool noPaging = false)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var authors = from s in db.Query<Author>()
                        select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                authors = authors.Where(s => s.Lastname.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    authors = authors.OrderByDescending(s => s.Lastname);
                    break;
                default:  // Name ascending 
                    authors = authors.OrderBy(s => s.Lastname);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(authors.ToPagedList(pageNumber, pageSize));
        }

        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Author author = db.Find<Author>(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            else
            {
                
                author.Books = db.Query<Book>().Where(b => b.Authors.Any(a => a.AuthorID == author.AuthorID)).ToList();
            }
            return View(author);
        }

        // GET: Authors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AuthorID,Lastname,Firstname")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.Add(author);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Find<Author>(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AuthorID,Lastname,Firstname")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.SetState(author, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Find<Author>(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Author author = db.Find<Author>(id);
            db.Remove(author);
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
