using Microsoft.EntityFrameworkCore;
using Resource.WebApi.Entity;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Infrastructure
{
    public class ResourceDbContext : DbContext
    {
        public ResourceDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RemoveFail> removeFails { get; set; }
    }
}
