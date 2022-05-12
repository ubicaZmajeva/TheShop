using System.Collections.Concurrent;
using Shop.WebApi.Models;

namespace Shop.WebApi.Services.Repositories;

public class Repository: IRepository
{
    private readonly ConcurrentDictionary<int, Article> _articles = new();
    public Article GetById(int id) => _articles.TryGetValue(id, out var article) ? article : null;
    public Article Save(Article article) => _articles.AddOrUpdate(article.Id, article, (_, _) => article);
}