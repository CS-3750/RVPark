using System.ComponentModel.DataAnnotations;

namespace RVPark.Core.Models
{
    public class EditUserModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}