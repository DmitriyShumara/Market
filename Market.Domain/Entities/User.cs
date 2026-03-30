namespace Market.Domain.Entities;

public class User
{
	public long Id { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public string? MiddleName { get; set; }
	public DateOnly? BirthDate { get; set; }

	public ICollection<Order> Orders { get; set; }
}