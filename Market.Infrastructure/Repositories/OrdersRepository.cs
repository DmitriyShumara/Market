using Market.Application.Repositories;
using Market.Domain;
using Market.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class OrdersRepository(MarketDbContext context) : IOrdersRepository
{
	public async Task<IEnumerable<Order>> GetAll(long userId, CancellationToken cancellationToken) =>
		await context.Orders
			.AsNoTracking()
			.Include(x => x.Items)
			.Where(x => x.UserId == userId)
			.ToListAsync(cancellationToken);
	
	public async Task<Order?> Get(long id, long userId, CancellationToken cancellationToken) =>
		await context.Orders
			.Include(x => x.Items)
			.ThenInclude(x => x.Product)
			.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

	public async Task Add(Order order, CancellationToken cancellationToken)
	{
		await context.Orders.AddAsync(order, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}
	
	public async Task Update(Order order, CancellationToken cancellationToken)
	{
		context.Orders.Update(order);
		await context.SaveChangesAsync(cancellationToken);
	}
}