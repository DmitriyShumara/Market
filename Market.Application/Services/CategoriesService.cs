using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Repositories;
using Market.Application.Services.Abstractions;
using Market.Domain.Entities;

namespace Market.Application.Services;

public class CategoriesService(ICategoriesRepository categoriesRepository, IProductsRepository productsRepository)
	: ICategoriesService
{
	public async Task<IEnumerable<CategoryListDto>> GetAll(CancellationToken cancellationToken)
	{
		var categories = await categoriesRepository.GetAll(cancellationToken);
		
		return GetCategoriesTree(categories);
	}
	
	public async Task Create(CategoryCreateDto request, CancellationToken cancellationToken)
	{
		var category = new Category
		{
			Name = request.Name
		};
		
		await categoriesRepository.Add(category, cancellationToken);
		
		var parentCategory = request.ParentId.HasValue
			? await categoriesRepository.Get(request.ParentId.Value, cancellationToken)
			: null;

		if (parentCategory is not null)
		{
			category.Path = $"{parentCategory.Path}.{category.Id:N}";
			
			await categoriesRepository.Update(category, cancellationToken);
		}
	}

	public async Task Update(Guid id, CategoryUpdateDto request, CancellationToken cancellationToken)
	{
		var category = await categoriesRepository.Get(id, cancellationToken);

		if (category is null)
		{
			throw new ArgumentNullException(nameof(category));
		}
		
		category.Name = request.Name;
		await categoriesRepository.Update(category, cancellationToken);
	}

	public async Task Delete(Guid id, CancellationToken cancellationToken)
	{
		if (await productsRepository.IsProductInCategory(id, cancellationToken))
		{
			throw new InvalidOperationException($"Cannot delete category with id {id}");
		}
		
		var category = await categoriesRepository.Get(id, cancellationToken);
		
		if (category is null)
		{
			throw new ArgumentNullException(nameof(category));
		}
		
		await categoriesRepository.Delete(category, cancellationToken);
	}
	
	private IEnumerable<CategoryListDto> GetCategoriesTree(IEnumerable<Category> categories)
	{
		var dictionary = categories.ToDictionary(
			category => category.Id,
			category => new CategoryListDto
			{
				Id = category.Id,
				Name = category.Name,
				Categories = []
			});

		var rootCategories = new List<CategoryListDto>();

		foreach (var category in categories)
		{
			if (category.Path.Contains('.'))
			{
				var parentPath = string.Join(".", category.Path.Split('.').SkipLast(1));
				var parentCategory = categories.FirstOrDefault(c => c.Path == parentPath);

				if (parentCategory != null && dictionary.TryGetValue(parentCategory.Id, out var parent))
				{
					parent.Categories.Add(dictionary[category.Id]);
				}
			}
			else
			{
				rootCategories.Add(dictionary[category.Id]);
			}
		}

		return rootCategories;
	}
}