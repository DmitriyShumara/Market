using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;
using Market.Domain.Enums;

namespace Market.Application.Services;

public class OrdersService(
	IOrdersRepository ordersRepository,
	IProductsRepository productsRepository,
	TimeProvider timeProvider)
	: IOrdersService
{
	public async Task<IEnumerable<OrderListDto>> GetAll(long userId, CancellationToken cancellationToken)
	{
		var orders = await ordersRepository.GetAll(userId, cancellationToken);

		return orders.Select(x => new OrderListDto
		{
			Id = x.Id,
			Price = x.Price,
			CreatedAt = x.CreatedAt
		});
	}

	public async Task<OrderDto> Get(long id, long userId, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.Get(id, userId, cancellationToken);

		if (order is null)
		{
			throw new ArgumentNullException(nameof(order));
		}

		return new OrderDto
		{
			Id = order.Id,
			Price = order.Price,
			CreatedAt = order.CreatedAt,
			Items = order.Items.Select(x => new OrderItemDto
			{
				Id = x.Id,
				Amount = x.Amount,
				Price = x.Price,
				Product = new OrderItemProductDto
				{
					Id = x.Product.Id,
					Name = x.Product.Name
				}
			})
		};
	}

	public async Task Create(long userId, OrderCreateDto request, CancellationToken cancellationToken)
	{
		var products = await productsRepository.Get(request.Items.Select(x => x.ProductId), cancellationToken);

		if (products.Any(x => x.Amount < request.Items.First(i => i.ProductId == x.Id).Amount))
		{
			throw new InvalidOperationException($"Some product has lower stock than requested");
		}

		var order = new Order
		{
			Status = OrderStatus.New,
			UserId = userId,
			Price = products.Sum(x => x.Price * request.Items.First(i => i.ProductId == x.Id).Amount),
			CreatedAt = timeProvider.GetLocalNow().DateTime,
			Items = products
				.Select(x => new OrderItem
				{
					ProductId = x.Id,
					Amount = request.Items.First(i => i.ProductId == x.Id).Amount,
					Price = x.Price
				})
				.ToList()
		};
		
		await ordersRepository.Add(order, cancellationToken);
		
		foreach (var product in products)
		{
			product.Amount -= request.Items.First(i => i.ProductId == product.Id).Amount;
		}
		
		await productsRepository.UpdateRange(products, cancellationToken);
	}

	public async Task Process(long id, long userId, CancellationToken cancellationToken) =>
		await ChangeOrderStatus(id, userId, OrderStatus.Processing, cancellationToken);
	
	public async Task Complete(long id, long userId, CancellationToken cancellationToken) =>
		await ChangeOrderStatus(id, userId, OrderStatus.Completed, cancellationToken);

	public async Task Cancel(long id, long userId, CancellationToken cancellationToken) =>
		await ChangeOrderStatus(id, userId, OrderStatus.Canceled, cancellationToken);

	private async Task ChangeOrderStatus(long id, long userId, OrderStatus status, CancellationToken cancellationToken)
	{
		var order = await ordersRepository.Get(id, userId, cancellationToken);
		
		if (order is null)
		{
			throw new ArgumentNullException(nameof(order));
		}
		
		order.Status = status;
		
		await ordersRepository.Update(order, cancellationToken);
	}
}