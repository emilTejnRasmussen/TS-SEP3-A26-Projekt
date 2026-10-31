using Entities;
using Microsoft.EntityFrameworkCore;
namespace Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}