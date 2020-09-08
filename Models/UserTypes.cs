using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MvcDriverVerify.Models
{
    public partial class UserTypes
    {
        public UserTypes()
        {
            User = new HashSet<User>();
        }

        public long UTId { get; set; }

        [Display(Name = "User Types")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [Required]
        public string UTDescription { get; set; }

        [Display(Name = "User(s)")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        public virtual ICollection<User> User { get; set; }
    }
}
