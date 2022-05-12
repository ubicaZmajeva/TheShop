using Shop.WebApi.Models;

namespace Shop.WebApi.Services.Cache;

public class CachedSupplier: ICachedSupplier
{
    private readonly Dictionary<int, Article> _cachedArticles = new();
    public bool ArticleInInventory(int id)
    {
        return _cachedArticles.ContainsKey(id);
    }

    public Article GetArticle(int id)
    {
        _cachedArticles.TryGetValue(id, out var article);
        return article;
    }

    public void SetArticle(Article article)
    {
        _cachedArticles.Add(article.Id, article);
    }
}