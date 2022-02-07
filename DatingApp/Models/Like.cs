using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Models
{
    [Table("Like")]
    public class Like
    {
        public string LikerId { get; set; }
        public string LikeeId { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}
