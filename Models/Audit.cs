using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Audit
    {
        public int Id { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        public string TableName { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}
