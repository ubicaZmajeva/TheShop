using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shop.WebApi.Commands;
using Shop.WebApi.Models;
using Shop.WebApi.Queries;

namespace Shop.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShopController : ControllerBase
{
    private readonly IMediator _mediator;
    public ShopController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetArticle(int id, int maxExpectedPrice = 200, CancellationToken cancellationToken = default) => 
        Ok(await _mediator.Send(GetArticleQuery.CreateInstance(id, maxExpectedPrice), cancellationToken));
    
    [HttpPost]
    public async Task<IActionResult> BuyArticle(Article article, int buyerId, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(BuyArticleCommand.CreateInstance(article, buyerId), cancellationToken);
            return Ok();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ApplicationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}