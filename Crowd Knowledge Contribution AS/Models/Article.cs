using Crowd_Knowledge_Contribution_AS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crowd_Knowledge_Contribution.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        public string ArticleTitle { get; set; }

        public DateTime LastModified { get; set; }

        [Required(ErrorMessage = "Selectati categoria")]
        public int CategoryId { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }   // un articol e scris de un utilizator

        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual Category Category { get; set; }

        public IEnumerable<SelectListItem> Categ { get; set; } // elementele desfasurate din meniul drop-down

    }
}