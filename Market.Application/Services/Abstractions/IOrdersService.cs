using Market.Application.Models.Requests;
using Market.Application.Models.Responses;

namespace Market.Application.Services.Abstractions;

public interface IOrdersService
{
	Task<IEnumerable<OrderListDto>> GetAll(long userId, CancellationToken cancellationToken);
	Task<OrderDto> Get(long id, long userId, CancellationToken cancellationToken);
	Task Create(long userId, OrderCreateDto request, CancellationToken cancellationToken);
	Task Process(long id, long userId, CancellationToken cancellationToken);
	Task Complete(long id, long userId, CancellationToken cancellationToken);
	Task Cancel(long id, long userId, CancellationToken cancellationToken);
}