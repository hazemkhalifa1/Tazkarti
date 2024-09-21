using System.ComponentModel.DataAnnotations;

namespace Tazkarti.Models
{
    public class UserVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Confirm Password Is Required")]
        [Compare("Password", ErrorMessage = "Confirm Pasword Dosen't Match Password")]
        public string ConfirmPassword { get; set; }
        public bool Agree { get; set; }

    }
}
