using MediatR;
using Shop.WebApi.Models;
using Shop.WebApi.Services;
using Shop.WebApi.Services.ArticleProviders.Core;
using Shop.WebApi.Services.Cache;

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
    
    
    public async Task<Article> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        if (_cachedSupplier.ArticleInInventory(request.Id))
        {
            var article = _cachedSupplier.GetArticle(request.Id);
            if (request.MaxExpectedPrice >= article.ArticlePrice)
            {
                return article;
            }
        }
        
        foreach (var articleProvider in _articleProviders)
        {
            if (!await articleProvider.ArticleInInventory(request.Id)) 
                continue;
            
            var article = await articleProvider.GetArticle(request.Id);
            if (request.MaxExpectedPrice < article.ArticlePrice) 
                continue;
            
            _cachedSupplier.SetArticle(article);
            return article;
        }
        return null;
    }
}