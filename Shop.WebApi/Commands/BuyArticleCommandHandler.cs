using MediatR;
using Shop.WebApi.Services.Repositories;

namespace Shop.WebApi.Commands;

public class BuyArticleCommandHandler : IRequestHandler<BuyArticleCommand>
{
    private readonly IRepository _db;
    private readonly ILogger _logger;

    public BuyArticleCommandHandler(IRepository db, ILogger<BuyArticleCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<Unit> Handle(BuyArticleCommand request, CancellationToken cancellationToken)
    {
        if (request.Article == null) throw new ArgumentNullException(nameof(request.Article), "Article is not provided");
        
        _logger.LogDebug("Trying to sell article with id {ArticleId}", request.Article.Id);
        request.Article.IsSold = true;
        request.Article.SoldDate = DateTime.Now;
        request.Article.BuyerUserId = request.BuyerId;
        
        try
        {
            _db.Save(request.Article);
            _logger.LogInformation("Article with id {ArticleId} is sold", request.Article.Id);
        }
        catch (ApplicationException ex)
        {
            _logger.LogError("Could not save article with id {ArticleId}", request.Article.Id);
            throw new ApplicationException($"Could not save article with id {request.Article.Id}", ex);
        }

        return Task.FromResult(Unit.Value);
    }
}