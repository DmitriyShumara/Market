using Market.Application.Services;
using Market.Application.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Application;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<ICategoriesService, CategoriesService>();
		services.AddScoped<IOrdersService, OrdersService>();
		services.AddScoped<IProductsService, ProductsService>();
		services.AddScoped<IUserService, UserService>();
		
		return services;
	}
}