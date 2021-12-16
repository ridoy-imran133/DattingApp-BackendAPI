using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("GetValues")]
        public List<Values> GetAllValues()
        {
            using (DattingAppDbContext _context = new DattingAppDbContext())
            {
                var val = _context.Values.OrderBy(x => x.Id).ToList();
                return val;
            }
               
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetValuesById")]
        public Values GetValueById(string id)
        {
            using (DattingAppDbContext _context = new DattingAppDbContext())
            {
                return _context.Values.FirstOrDefault(x => x.Id == id);
            }
        }
    }
}