using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Models
{
    [Table("Value")]
    public class Values
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
