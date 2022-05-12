using Vendor.WebApi.Models;

namespace Vendor.WebApi.Services;

public class SupplierService: ISupplierService
{
    public bool ArticleInInventory(int id) => new Random().NextDouble() >= 0.5;

    public Article GetArticle(int id) =>
        new()
        {
            Id = id,
            NameOfArticle = $"Article {id}",
            ArticlePrice = new Random().Next(100, 500)
        };
}