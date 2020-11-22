using Crowd_Knowledge_Contribution_AS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Crowd_Knowledge_Contribution.Models
{
    public class Chapter
    {
        [Key]
        public int ChapterId { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        public string ChapterTitle { get; set; }

        [Required(ErrorMessage = "Continutul este obligatoriu")]
        [DataType(DataType.MultilineText)]
        public string ChapterContent { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }   // un articol e scris de un utilizator


        [Required]
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}