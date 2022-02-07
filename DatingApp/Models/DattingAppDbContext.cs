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
        public virtual DbSet<Like> Likes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(k => new { k.LikerId, k.LikeeId });

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
