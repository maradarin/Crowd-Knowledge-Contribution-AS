using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Crowd_Knowledge_Contribution_AS.Models
{
    public class Modification
    {
        [Key]
        public int ModificationId { get; set; }

        [Required]
        public int ComponentId { get; set; }

        public string ModifiedField { get; set; }
        public string OldInfo { get; set; }
        public string NewInfo { get; set; }

        public string ModifiedController { get; set; }

        public DateTime LastModified { get; set; }
    }
}