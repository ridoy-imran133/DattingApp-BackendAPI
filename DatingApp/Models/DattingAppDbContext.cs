using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Models
{
    public class DattingAppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DattingAppDbContext()
        {

        }
        public DattingAppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DattingAppDbContext(DbContextOptions<DattingAppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Values> Values { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
