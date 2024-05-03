using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModels
{
    public class CustomLoginRequestViewModel
    {
        [Required]
        [RegularExpression("^[\\w-\\.+]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
