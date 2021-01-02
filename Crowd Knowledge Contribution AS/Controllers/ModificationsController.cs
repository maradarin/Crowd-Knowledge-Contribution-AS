using Crowd_Knowledge_Contribution_AS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crowd_Knowledge_Contribution_AS.Controllers
{
    public class ModificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Modifications
        public ActionResult IndexArticle(int id)
        {
            var modifications = db.Modifications.Where(m => m.ComponentId == id);
            ViewBag.Modifications = modifications;
            return View();
        }

        public ActionResult IndexChapter(int id)
        {
            var modificationsTitle = db.Modifications.Where(m => m.ComponentId == id && m.ModifiedField == "ChapterTitle");
            ViewBag.ModificationsTitle = modificationsTitle;

            var modificationsContent = db.Modifications.Where(m => m.ComponentId == id && m.ModifiedField == "ChapterContent");
            ViewBag.ModificationsContent = modificationsContent;
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {

            Modification modification = db.Modifications.First(m => m.ModificationId == id);
            db.Modifications.Remove(modification);
            db.SaveChanges();
            if(modification.ModifiedField == "ArticleTitle")
            {
                Crowd_Knowledge_Contribution.Models.Article article = db.Articles.First(c => c.ArticleId == modification.ComponentId);

                return RedirectToAction("Update", "Articles", new { id = modification.ComponentId, info = modification.OldInfo, idModif = modification.ModificationId });
            }
            else if(modification.ModifiedField == "ChapterTitle")
            {
                Crowd_Knowledge_Contribution.Models.Chapter chapter = db.Chapters.First(c => c.ChapterId == modification.ComponentId);

                return RedirectToAction("Update", "Chapters", new { id = modification.ComponentId, field = modification.ModifiedField, info = modification.OldInfo, idModif = modification.ModificationId });
            }
            else
            {
                Crowd_Knowledge_Contribution.Models.Chapter chapter = db.Chapters.First(c => c.ChapterId == modification.ComponentId);

                return RedirectToAction("Update", "Chapters", new { id = modification.ComponentId, field = modification.ModifiedField, info = modification.OldInfo, idModif = modification.ModificationId });

            }

            TempData["message"] = "Modificarea a fost stearsa";
            return RedirectToAction("Index");
        }
    }
}