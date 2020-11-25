using Crowd_Knowledge_Contribution.Models;
using Crowd_Knowledge_Contribution_AS.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crowd_Knowledge_Contribution.Controllers
{
    public class ChaptersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Chapters
        public ActionResult Index()
        {
            //var chapters = db.Chapters.Include("Article");
            var chapters = from chapter in db.Chapters
                           select chapter;
            ViewBag.Chapters = chapters;
            return View();
        }

        public ActionResult Show(int id)
        {
            Chapter chapter = db.Chapters.Include("User").First(m => m.ChapterId == id);
            Debug.WriteLine(chapter.User == null);
            //ViewBag.Chapter = chapter;
            //ViewBag.Article = chapter.Article;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(chapter);
        }

        public ActionResult New(int id)
        {
            Chapter chapter = new Chapter();
            chapter.ArticleId = id;
            chapter.UserId = User.Identity.GetUserId();
            return View(chapter);
        }

        [HttpPost]
        public ActionResult New(Chapter chapter)
        {
            chapter.UserId = User.Identity.GetUserId();
            try
            {
                if(ModelState.IsValid)
                {
                    //chapter.UserId = User.Identity.GetUserId();
                    db.Chapters.Add(chapter);
                    db.SaveChanges();
                    TempData["mesage"] = "Capitolul a fost adaugat";
                    return Redirect("/Articles/Show/" + chapter.ArticleId.ToString());
                }
                else
                {
                    return View(chapter);
                }
            }
            catch (Exception e)
            {
                return View(chapter);
            }
        }

        public ActionResult Edit(int id)
        {
            Chapter chapter = db.Chapters.Include("Article").Include("User").First(m => m.ChapterId == id);
            if (chapter.User.UserName == System.Web.HttpContext.Current.User.Identity.Name || User.IsInRole("Admin"))
            {
                //chapter.ArticleId = (from chapter_db in db.Chapters
                //                     where chapter_db.ChapterId == id
                //                     select chapter_db.ArticleId).FirstOrDefault();
                return View(chapter);
            }
            //ViewBag.Chapter = chapter;
            return RedirectToAction("Index");
        }

        [HttpPut]
        public ActionResult Edit(int id, Chapter requestChapter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Chapter chapter = db.Chapters.Include("User").First(m => m.ChapterId == id);
                    if (chapter.User.UserName == System.Web.HttpContext.Current.User.Identity.Name || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(chapter))
                        {
                            chapter.ChapterTitle = requestChapter.ChapterTitle;
                            chapter.ChapterContent = requestChapter.ChapterContent;
                            TempData["message"] = "Capitolul a fost editat";
                            db.SaveChanges();
                            return Redirect("/Chapters/Show/" + chapter.ChapterId.ToString());
                        }
                        else
                        {
                            return View(requestChapter);
                        }
                    }
                    else
                        return View(requestChapter);
                }
                else
                {
                    return View(requestChapter);
                }
            }
            catch (Exception e)
            {
                return View(requestChapter);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Chapter chapter = db.Chapters.Include("User").First(m => m.ChapterId == id);
            if (chapter.User.UserName == System.Web.HttpContext.Current.User.Identity.Name || User.IsInRole("Admin"))
            {
                db.Chapters.Remove(chapter);
                db.SaveChanges();
            }
            return Redirect("/Articles/Show/" + chapter.ArticleId.ToString());
        }
    }
}