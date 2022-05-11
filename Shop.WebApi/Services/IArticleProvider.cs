using Shop.WebApi.Models;

namespace Shop.WebApi.Services;

public interface IArticleProvider
{
    bool ArticleInInventory(int id);
    Article GetArticle(int id);
}