using System.Reflection;
using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure;

public class MarketDbContext(DbContextOptions<MarketDbContext> options) : DbContext(options)
{
	public DbSet<Category> Categories { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<User> Users { get; set; }
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);
	}
}