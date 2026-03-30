namespace Market.Domain.Entities;

public class Category
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public string? Path { get; set; }

	public ICollection<Product> Products { get; set; }
}