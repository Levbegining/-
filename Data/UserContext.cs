using System;
using Microsoft.EntityFrameworkCore;
using ProjectUser.Models;

namespace ProjectUser.Data;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserContext(DbContextOptions<UserContext> options) : base(options){ }
}
