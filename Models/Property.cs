using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Property : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Detail { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsTrending { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
