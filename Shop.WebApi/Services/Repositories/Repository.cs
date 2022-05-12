using System.Collections.Concurrent;
using Shop.WebApi.Models;

namespace Shop.WebApi.Services.Repositories;

public class Repository: IRepository
{
    private readonly ConcurrentDictionary<int, Article> _articles = new();
    public Article GetById(int id) => _articles.TryGetValue(id, out var article) ? article : null;
    public Article Save(Article article) =>
        _articles.TryAdd(article.Id, article)
            ? article
            : throw new ApplicationException($"Entity with id {article.Id}, already exists");
}