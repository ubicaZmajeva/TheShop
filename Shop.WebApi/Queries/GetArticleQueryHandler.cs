using MediatR;
using Shop.WebApi.Models;
using Shop.WebApi.Services;

namespace Shop.WebApi.Queries;

public class GetArticleQueryHandler : IRequestHandler<GetArticleQuery, Article>
{
    private readonly ICachedSupplier _cachedSupplier;
    private readonly IEnumerable<IArticleProvider> _articleProviders;
    
        
    public GetArticleQueryHandler(
        ICachedSupplier cachedSupplier, IEnumerable<IArticleProvider> articleProviders)
    {
        _cachedSupplier = cachedSupplier;
        _articleProviders = articleProviders;
    }
    
    
    public Task<Article> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        if (_cachedSupplier.ArticleInInventory(request.Id))
        {
            var article = _cachedSupplier.GetArticle(request.Id);
            if (request.MaxExpectedPrice >= article.ArticlePrice)
            {
                return Task.FromResult(article);
            }
        }
        
        foreach (var articleProvider in _articleProviders)
        {
            if (!articleProvider.ArticleInInventory(request.Id)) 
                continue;
            
            var article = articleProvider.GetArticle(request.Id);
            if (request.MaxExpectedPrice < article.ArticlePrice) 
                continue;
            
            _cachedSupplier.SetArticle(article);
            return Task.FromResult(article);
        }
        return Task.FromResult<Article>(null);
    }
}