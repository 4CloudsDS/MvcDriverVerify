using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcDriverVerify.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            User = new HashSet<User>();
        }

        public long VId { get; set; }

        [Display(Name = "Registration Number")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [Required]
        public string Vregistration { get; set; }

        [Display(Name = "Make")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [Required]
        public string VMake { get; set; }

        [Display(Name = "Model")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [Required]
        public string VModelName { get; set; }

        [Display(Name = "Year")]    
        [Required]
        [Range(1990, 2020)]        
        public int VModelYear { get; set; }

        [Display(Name = "Driver")]  
        public long? VParterId { get; set; }

        [Display(Name = "Platform ID")]  
        public long? VPlatformId { get; set; }

        [Display(Name = "Vehicle Driver")]  
        public virtual User VParter { get; set; }

        [Display(Name = "Vehicle Platform")]  
        public virtual Platform VPlatform { get; set; }

        [Display(Name = "Vehicle Driver")]  
        public virtual ICollection<User> User { get; set; }
    }
}
