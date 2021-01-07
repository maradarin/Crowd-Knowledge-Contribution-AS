using Crowd_Knowledge_Contribution.Models;
using Crowd_Knowledge_Contribution_AS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crowd_Knowledge_Contribution_AS.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            List<Category> categories = db.Categories.ToList();
            List<Article> articles = new List<Article>();
            foreach (Category category in categories)
            {
                if (db.Articles.Where(a => a.CategoryId == category.CategoryId).Count() == 0)
                {
                    ViewBag.Message = "No new articles";
                }
                else
                {
                    Article article = db.Articles.OrderByDescending(a => a.LastModified).First(a => a.CategoryId == category.CategoryId);
                    articles.Add(article);
                }
            }
            ViewBag.Categories = categories;
            ViewBag.Articles = articles;
            return View();
        }
        public ActionResult NotFound()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Show(int id, string sortOrder)
        {
            List<Article> articles = db.Articles.Where(a => a.CategoryId == id).ToList();
            Category category = db.Categories.First(c => c.CategoryId == id);
            switch (sortOrder)
            {
                case "articleTitle":
                    articles = db.Articles.Include("Category").Include("User").Where(a => a.CategoryId == id).OrderByDescending(a => a.ArticleTitle).ToList();
                    break;
                case "lastModified":
                    articles = db.Articles.Include("Category").Include("User").Where(a => a.CategoryId == id).OrderByDescending(a => a.LastModified).ToList();
                    break;
                case "userName":
                    articles = db.Articles.Include("Category").Include("User").Where(a => a.CategoryId == id).OrderByDescending(a => a.User.UserName).ToList();
                    break;
                default:
                    articles = db.Articles.Include("Category").Include("User").Where(a => a.CategoryId == id).OrderByDescending(a => a.ArticleTitle).ToList();
                    break;
            }
            ViewBag.Articles = articles;
            ViewBag.Category = category;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}