using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Category : BaseEntity
    {

        [Required(ErrorMessage = "Please, provide a name")]
        [MaxLength(20)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please, provide a url for the category's image")]
        public string ImageUrl { get; set; }
        [MaxLength(100, ErrorMessage = "Only 100 characteres or below are supported for description")]
        public string Description { get; set; }

        public List<Property> Properties{ get; set; }
    }
}
