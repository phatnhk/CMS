﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace cms.Data
{
    public class CMSDbContextFactory : IDesignTimeDbContextFactory<CMSDbContext>
    {
        public CMSDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();
            var builder = new DbContextOptionsBuilder<CMSDbContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new CMSDbContext(builder.Options);
        }
    }
}
