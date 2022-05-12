using Shop.WebApi.Models;

namespace Shop.WebApi.Services.Cache;

public interface ICachedSupplier
{
    bool ArticleInInventory(int id);
    Article GetArticle(int id);
    Article SetArticle(Article article);
}