using Shop.WebApi.Models;
using Shop.WebApi.Services.Repositories;

namespace Shop.WebApi.Services.Cache;

public class CachedSupplier: ICachedSupplier
{
    private readonly IRepository _repository;
    public CachedSupplier(IRepository repository)
    {
        _repository = repository;
    }

    public bool ArticleInInventory(int id) => GetArticle(id) != null;
    public Article GetArticle(int id) => _repository.GetById(id);
    public Article SetArticle(Article article) => _repository.Save(article);
}