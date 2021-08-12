using System.ComponentModel.DataAnnotations;

namespace Emailer.Models
{
    public class ContactUsRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Message { get; set; }

        [Required]
        [MaxLength(2000)]
        public string HowSoon { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }
    }
}
