using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPI.Data.Entities;

namespace UserAPI.Data.DAL
{
    public class UserDbContext: IdentityDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
