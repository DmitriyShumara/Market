using Market.Application.Models.Requests;
using Market.Application.Models.Responses;
using Market.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Route("api/users/{userId:long}/orders")]
public class OrdersController(IOrdersService service) : ApiController
{
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<OrderListDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll([FromRoute] long userId, CancellationToken cancellationToken)
	{
		var orders = await service.GetAll(userId, cancellationToken);

		return Ok(orders);
	}
	
	[HttpGet("{id:long}")]
	[ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
	{
		var order = await service.Get(id, userId, cancellationToken);

		return Ok(order);
	}
	
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Create([FromRoute] long userId, [FromBody] OrderCreateDto request, CancellationToken cancellationToken)
	{
		await service.Create(userId, request, cancellationToken);

		return Created();
	}
	
	[HttpPut("{id:long}/process")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Process([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
	{
		await service.Process(userId, id, cancellationToken);

		return NoContent();
	}
	
	[HttpPut("{id:long}/complete")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Complete([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
	{
		await service.Complete(userId, id, cancellationToken);

		return NoContent();
	}
	
	[HttpDelete("{id:long}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	public async Task<IActionResult> Delete([FromRoute] long userId, [FromRoute] long id, CancellationToken cancellationToken)
	{
		await service.Cancel(id, userId, cancellationToken);

		return NoContent();
	}
}