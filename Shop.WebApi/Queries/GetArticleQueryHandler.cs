using MediatR;
using Shop.WebApi.Models;
using Shop.WebApi.Services.ArticleProviders.Core;
using Shop.WebApi.Services.Cache;

namespace Shop.WebApi.Queries;

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, Article>
{
    private readonly ICachedSupplier _cachedSupplier;
    private readonly IEnumerable<IArticleProvider> _articleProviders;
    private readonly ILogger<GetArticleQueryHandler> _logger;


    public GetArticleQueryHandler(ICachedSupplier cachedSupplier, 
        IEnumerable<IArticleProvider> articleProviders,
        ILogger<GetArticleQueryHandler> logger)
    {
        _cachedSupplier = cachedSupplier;
        _articleProviders = articleProviders;
        _logger = logger;
    }
    
    public async Task<Article> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        if (_cachedSupplier.ArticleInInventory(request.Id))
        {
            var article = _cachedSupplier.GetArticle(request.Id);
            if (article == null || request.MaxExpectedPrice >= article.ArticlePrice)
            {
                _logger.LogTrace("Cache hit for article {ArticleId}. Returning item from cache... ", request.Id);
                return article;
            }
        }
        
        foreach (var articleProvider in _articleProviders)
        {
            _logger.LogTrace("Checking availability of article at provider {ProviderType}", articleProvider.GetType().Name);
            if (!await articleProvider.ArticleInInventory(request.Id))
            {
                _logger.LogTrace("Provider cannot deliver article");
                continue;
            }

            var article = await articleProvider.GetArticle(request.Id);
            if (article == null || request.MaxExpectedPrice < article.ArticlePrice)
            {
                _logger.LogTrace("There is an error on provider side, or article is too expensive");
                continue;
            }
            
            _logger.LogTrace("We have it... put it in cache and return it...");
            _cachedSupplier.SetArticle(article);
            return article;
        }
        return null;
    }
}