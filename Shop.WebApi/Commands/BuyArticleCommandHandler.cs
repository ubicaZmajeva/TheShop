using MediatR;
using Shop.WebApi.Services;
using Shop.WebApi.Services.Repositories;

namespace Shop.WebApi.Commands;

public class BuyArticleCommandHandler : RequestHandler<BuyArticleCommand>
{
    private readonly IRepository _db;
    private readonly ILogger _logger;

    public BuyArticleCommandHandler(IRepository db, ILogger<BuyArticleCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    protected override void Handle(BuyArticleCommand request)
    {
        if (request.Article == null)
        {
            throw new Exception("Could not order article");
        }
        
        _logger.LogDebug("Trying to sell article with id {ArticleId}", request.Article.Id);
        
        request.Article.IsSold = true;
        request.Article.SoldDate = DateTime.Now;
        request.Article.BuyerUserId = request.BuyerId;
        
        try
        {
            _db.Save(request.Article);
            _logger.LogInformation("Article with id {ArticleId} is sold", request.Article.Id);
        }
        catch (ArgumentNullException)
        {
            _logger.LogError("Could not save article with id {ArticleId}", request.Article.Id);
            throw new Exception("Could not save article with id");
        }
        catch (Exception)
        {
            // ignored
        }
    }
}