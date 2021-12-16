using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.DTOS
{
    public class UserForLoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
