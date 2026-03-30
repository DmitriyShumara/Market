using Market.Domain;
using Market.Domain.Entities;

namespace Market.Application.Repositories;

public interface IOrdersRepository
{
	Task<IEnumerable<Order>> GetAll(long userId, CancellationToken cancellationToken);
	Task<Order?> Get(long id, long userId, CancellationToken cancellationToken);
	Task Add(Order order, CancellationToken cancellationToken);
	Task Update(Order order, CancellationToken cancellationToken);
}