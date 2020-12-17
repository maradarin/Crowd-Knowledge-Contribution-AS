﻿using Crowd_Knowledge_Contribution.Models;
using Crowd_Knowledge_Contribution_AS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            var search = "";
            if(Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<int> articleIds = db.Articles.Where(at => at.ArticleTitle.Contains(search)).Select(a => a.ArticleId).ToList();
                List<int> chapterIds = db.Chapters.Where(c => c.ChapterContent.Contains(search)).Select(ch => ch.ArticleId).ToList();
                List<int> chapterIds2 = db.Chapters.Where(c => c.ChapterTitle.Contains(search)).Select(ch => ch.ArticleId).ToList();
                //Search by category
                /******* needs work *******/
                //List<int> categoryIds = db.Categories.Where(cat => cat.CategoryName.Contains(search)).Select(ct => ct.CategoryId).ToList();
                List<int> mergedIds = articleIds.Union(chapterIds).Union(chapterIds2).ToList();
                articles = (System.Data.Entity.Infrastructure.DbQuery<Article>)db.Articles.Where(article => mergedIds.Contains(article.ArticleId)).Include("Category").Include("User").OrderBy(a => a.LastModified);
            }

            var totalItems = articles.Count();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.total = totalItems;
            ViewBag.Articles = articles;
            ViewBag.SearchString = search;
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
        [Authorize(Roles = "Editor,Admin")]
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
            Article article = db.Articles.Include("User").First(m => m.ArticleId == id);
            if (article.User.UserName == System.Web.HttpContext.Current.User.Identity.Name || User.IsInRole("Admin"))
            {
                article.Categ = GetAllCategories();
                return View(article);
            }
            return RedirectToAction("Index");
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
                        Article article = db.Articles.Include("User").First(m => m.ArticleId == id);
                        if (article.User.UserName == System.Web.HttpContext.Current.User.Identity.Name || User.IsInRole("Admin"))
                        {
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
                    else
                    {
                        return View(requestArticle);
                    }

                }
                catch (Exception e)
                {
                    return View(requestArticle);
                }
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {

            Article article = db.Articles.Include("User").First(m => m.ArticleId == id);
            if (article.User.UserName == System.Web.HttpContext.Current.User.Identity.Name || User.IsInRole("Admin"))
            {
                db.Articles.Remove(article);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters";
                return RedirectToAction("Index");
            }

            return View();
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