using Shop.WebApi.Models;

namespace Shop.WebApi.Services;

public class Warehouse: IWarehouse
{
    public bool ArticleInInventory(int id)
    {
        return new Random().NextDouble() >= 0.5;
    }

    public Article GetArticle(int id)
    {
        return new Article()
        {
            Id = id,
            NameOfArticle = $"Article {id}",
            ArticlePrice = new Random().Next(100, 500)
        };
    }
}