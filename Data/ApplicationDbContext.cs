﻿using Key_Management_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Key_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }
        
        public DbSet<User> users { get; set; }
        public DbSet<Worker> workers { get; set; }
        public DbSet<KeyCollector> keyCollectors { get; set; }
        public DbSet<Key> Key {  get; set; }    
        public DbSet<RequestKey> RequestKey { get; set; }
        public DbSet<ThirdParty> ThirdParty { get; set; }
        public DbSet<LogoutToken> LogoutTokens { get; set; }
        public override DbSet<Role> Roles {  get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
