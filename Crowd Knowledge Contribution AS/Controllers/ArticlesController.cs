using Crowd_Knowledge_Contribution.Models;
using Crowd_Knowledge_Contribution_AS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crowd_Knowledge_Contribution.Controllers
{
    public class ArticlesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Categories
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Index()
        {
            var articles = db.Articles.Include("Category").Include("User");
            ViewBag.Articles = articles;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {
            Article article = db.Articles.Include("User").First(m => m.ArticleId == id);
            //ViewBag.Article = article;
            //ViewBag.Chapters = article.Chapters;
            //ViewBag.Category = article.Category;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(article);
        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New()
        {
            Article article = new Article();
            article.Categ = GetAllCategories();

            // Preluam ID-ul utilizatorului curent
            article.UserId = User.Identity.GetUserId();
            return View(article);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult New(Article article)
        {
            article.LastModified = DateTime.Now;
            article.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Articles.Add(article);
                    db.SaveChanges();
                    TempData["mesage"] = "Articolul a fost adaugat";
                    return RedirectToAction("Index"); // daca totul e bine, mergem sa vedem articolul nou adaugat
                }
                else
                {
                    article.Categ = GetAllCategories();
                    return View(article);
                }
            }
            catch (Exception e)
            {
                article.Categ = GetAllCategories();
                return View(article);
            }
        }

        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Article article = db.Articles.Find(id);
            article.Categ = GetAllCategories();
            return View(article);
        }

        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id, Article requestArticle)
        {
            requestArticle.Categ = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    Article article = db.Articles.Find(id);

                    if (TryUpdateModel(article))
                    {
                        //article = requestArticle;
                        article.ArticleTitle = requestArticle.ArticleTitle;
                        //article.LastModified = requestArticle.LastModified;
                        article.CategoryId = requestArticle.CategoryId;
                        db.SaveChanges();
                        TempData["message"] = "Articolul a fost modificat!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestArticle);
                }

            }
            catch (Exception e)
            {
                return View(requestArticle);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {

            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            TempData["message"] = "Articolul a fost sters";
            return RedirectToAction("Index");
        }

        [NonAction] // metoda nu va putea fi accesata din ruta
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // extragem o lista goala
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pt drop-down
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            // SAU
            /*foreach(var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.CategoryId.ToString();
                listItem.Text = category.CategoryName.ToString();

                selectList.Add(listItem);
            }*/

            // returnam lista de categorii
            return selectList;
        }
    }
}