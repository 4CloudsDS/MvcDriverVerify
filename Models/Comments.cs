using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcDriverVerify.Models
{
    public partial class Comments
    {

        public long CId { get; set; }
   
        [StringLength(60, MinimumLength = 10)]
        [Required]
        [Display(Name = "Comment")]
        public string CText { get; set; }
 
        [Display(Name = "Comment Time")]
        [DataType(DataType.Date)]
        public DateTime CDateTime { get; set; }

        [Display(Name = "Commentor")]
        public long? CUid { get; set; }

        [Display(Name = "Who is the comment about?")]
        public long? CPid { get; set; }


        [Display(Name = "Subject")]
        public virtual User CP { get; set; }

        [Display(Name = "Commentor")]
        public virtual User CU { get; set; }
    }
}
