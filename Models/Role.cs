using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Role : BaseEntity
    {
        public string Description { get; set; }
        public List<User> Users { get; set; }
    }
}
