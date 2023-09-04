using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        // Password = Hash Ciphertext password
        [Required]
        [MaxLength(128)]
        [MinLength(6)]
        public string Password { get; set; }

        [MaxLength(128)]
        public string PasswordSalt { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }
        public List<Property> Properties { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
