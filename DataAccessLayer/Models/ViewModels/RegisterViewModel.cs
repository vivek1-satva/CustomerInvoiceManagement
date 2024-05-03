using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression("^[\\w-\\.+]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Invalid email")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression("^[a-zA-Z]+(?: [a-zA-Z]+)*$", ErrorMessage = "Invalid Name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid Phone Number")]
        public string Number { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
