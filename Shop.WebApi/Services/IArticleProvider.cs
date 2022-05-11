using Shop.WebApi.Models;

namespace Shop.WebApi.Services;

public interface IArticleProvider
{
    bool ArticleInInventory(int id);
    Article GetArticle(int id);
}

public interface IWarehouse : IArticleProvider
{
}

public interface IDealer1 : IArticleProvider
{
}

public interface IDealer2 : IArticleProvider
{
}
