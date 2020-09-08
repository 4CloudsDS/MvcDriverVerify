using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcDriverVerify.Models
{
    public partial class Platform
    {
        public Platform()
        {
            User = new HashSet<User>();
            Vehicle = new HashSet<Vehicle>();
        }

        public long PId { get; set; }

        [Display(Name = "Platform Name")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        public string PName { get; set; }

        [Display(Name = "Platform Type")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string PType { get; set; }

        [Display(Name = "Platform User(s)")] 
        public virtual ICollection<User> User { get; set; }

        [Display(Name = "Platform Vehicle(s)")] 
        public virtual ICollection<Vehicle> Vehicle { get; set; }
    }
}
