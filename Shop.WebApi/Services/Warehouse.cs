using Shop.WebApi.Models;

namespace Shop.WebApi.Services;

public class Warehouse: IArticleProvider
{
    public Task<bool> ArticleInInventory(int id)
    {
        return Task.FromResult(new Random().NextDouble() >= 0.5);
    }

    public Task<Article> GetArticle(int id)
    {
        return Task.FromResult(new Article()
        {
            Id = id,
            NameOfArticle = $"Article {id}",
            ArticlePrice = new Random().Next(100, 500)
        });
    }
}