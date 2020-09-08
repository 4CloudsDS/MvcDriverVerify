using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcDriverVerify.Models
{
    public partial class User
    {
        public User()
        {
            CommentsCP = new HashSet<Comments>();
            CommentsCU = new HashSet<Comments>();
            InverseUPartner = new HashSet<User>();
            Vehicle = new HashSet<Vehicle>();
        }

        public long UId { get; set; }

        [Display(Name = "User Name")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [Required]
        public string UNames { get; set; }

        [Display(Name = "Gender")]    
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Required]
        public string UGender { get; set; }

        [Display(Name = "Age: (18 - 75)")]    
        [Range(1, 100)]
        [Required]
        public int UAge { get; set; }

        [Display(Name = "Rating: (0 - 5)")]    
        [Range(1, 100)]
        [Required]
        public decimal URating { get; set; }

        [Display(Name = "Vehicle ID")] 
        public long? UVid { get; set; }

        [Display(Name = "Partner ID")] 
        public long? UPartnerId { get; set; }

        [Display(Name = "User Type ID")] 
        [Required]
        public long? UUsertypeId { get; set; }

        [Display(Name = "Platform ID")] 
        public long? UPlatformId { get; set; }

        [Display(Name = "Partner")] 
        public virtual User UPartner { get; set; }

        [Display(Name = "Platform")] 
        public virtual Platform UPlatform { get; set; }

        [Display(Name = "User Type")] 
        public virtual UserTypes UUsertype { get; set; }

        [Display(Name = "Vehicle")] 
        public virtual Vehicle UV { get; set; }

        [Display(Name = "User(s) Commented Comments")] 
        public virtual ICollection<Comments> CommentsCP { get; set; }

        [Display(Name = "User ID")] 
        public virtual ICollection<Comments> CommentsCU { get; set; }

        [Display(Name = "Partner(s)")] 
        public virtual ICollection<User> InverseUPartner { get; set; }

        [Display(Name = "Vehicle(s)")] 
        public virtual ICollection<Vehicle> Vehicle { get; set; }
    }
}
