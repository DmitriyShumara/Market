using Market.Domain.Enums;

namespace Market.Domain.Entities;

public class Order
{
	public long Id { get; set; }
	public long UserId { get; set; }
	public OrderStatus Status { get; set; }
	public decimal Price { get; set; }
	public DateTime CreatedAt { get; set; }

	public ICollection<OrderItem> Items { get; set; }
	public User User { get; set; }
}