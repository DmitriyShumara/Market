using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;

namespace Market.Application.Services;

public class ProductsService(IProductsRepository repository) : IProductsService
{
	public async Task<IEnumerable<ProductListDto>> GetAll(Guid? categoryId, CancellationToken cancellationToken)
	{
		var products = await repository.GetAll(categoryId, cancellationToken);

		return products.Select(x => new ProductListDto
		{
			Id = x.Id,
			Name = x.Name,
			Category = new ProductListCategoryDto
			{
				Id = x.CategoryId,
				Name = x.Category.Name
			},
			Amount = x.Amount,
			Price = x.Price
		});
	}
	
	public async Task<ProductListDto> Get(long id, CancellationToken cancellationToken)
	{
		var product = await repository.Get(id, cancellationToken);

		if (product is null)
		{
			throw new ArgumentNullException(nameof(product));
		}

		return new ProductListDto
		{
			Id = product.Id,
			Name = product.Name,
			Category = new ProductListCategoryDto
			{
				Id = product.CategoryId,
				Name = product.Category.Name
			},
			Amount = product.Amount,
			Price = product.Price
		};
	}

	public async Task Create(ProductCreateDto request, CancellationToken cancellationToken)
	{
		var product = new Product
		{
			Name = request.Name,
			CategoryId = request.CategoryId,
			Amount = request.Amount,
			Price = request.Price,
			IsDeleted = false
		};
		
		await repository.Add(product, cancellationToken);
	}
	
	public async Task Update(long id, ProductCreateDto request, CancellationToken cancellationToken)
	{
		var product = await repository.Get(id, cancellationToken);
		
		if (product is null)
		{
			throw new ArgumentNullException(nameof(product));
		}
		
		product.Name = request.Name;
		product.CategoryId = request.CategoryId;
		product.Amount = request.Amount;
		product.Price = request.Price;
		
		await repository.Update(product, cancellationToken);
	}
	
	public async Task Delete(long id, CancellationToken cancellationToken)
	{
		var product = await repository.Get(id, cancellationToken);
		
		if (product is null)
		{
			throw new ArgumentNullException(nameof(product));
		}
		
		product.IsDeleted = true;
		
		await repository.Update(product, cancellationToken);
	}
}