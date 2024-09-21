using System.ComponentModel.DataAnnotations;

namespace Tazkarti.Models
{
    public class LogUserVM
    {
        [Required]
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
