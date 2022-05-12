using Shop.WebApi.Models;

namespace Shop.WebApi.Services;

public interface IArticleProvider
{
    Task<bool> ArticleInInventory(int id);
    Task<Article> GetArticle(int id);
}
