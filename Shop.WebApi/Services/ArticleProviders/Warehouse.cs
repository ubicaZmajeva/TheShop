using Shop.WebApi.Models;
using Shop.WebApi.Services.ArticleProviders.Core;

namespace Shop.WebApi.Services.ArticleProviders;

public class Warehouse: IArticleProvider
{
    public Task<bool> ArticleInInventory(int id) => Task.FromResult(new Random().NextDouble() >= 0.5);

    public Task<Article> GetArticle(int id) =>
        Task.FromResult(new Article
        {
            Id = id,
            NameOfArticle = $"Article {id}",
            ArticlePrice = new Random().Next(100, 500)
        });
}