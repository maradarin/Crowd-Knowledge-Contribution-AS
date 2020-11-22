using Crowd_Knowledge_Contribution.Models;
using Crowd_Knowledge_Contribution_AS.Models;
using System;
using System.Collections.Generic;
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
            Chapter chapter = db.Chapters.Find(id);
            ViewBag.Chapter = chapter;
            ViewBag.Article = chapter.Article;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        public ActionResult New(int id)
        {
            Chapter chapter = new Chapter();
            chapter.ArticleId = id;
            return View(chapter);
        }

        [HttpPost]
        public ActionResult New(Chapter chapter)
        {
            try
            {
                if(ModelState.IsValid)
                {
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
            Chapter chapter = db.Chapters.Find(id);
            chapter.ArticleId = (from chapter_db in db.Chapters
                                 where chapter_db.ChapterId == id
                                 select chapter_db.ArticleId).FirstOrDefault();
            //ViewBag.Chapter = chapter;
            return View(chapter);
        }

        [HttpPut]
        public ActionResult Edit(int id, Chapter requestChapter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Chapter chapter = db.Chapters.Find(id);
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
            Chapter chapter = db.Chapters.Find(id);
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            return Redirect("/Articles/Show/" + chapter.ArticleId.ToString());
        }
    }
}