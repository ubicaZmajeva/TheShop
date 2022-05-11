#nullable enable
using Microsoft.AspNetCore.Mvc;
using Shop.WebApi.Models;
using Shop.WebApi.Services;

namespace Shop.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShopController : ControllerBase
{
    private readonly IRepository _db;
    private readonly ICachedSupplier _cachedSupplier;
    private readonly IEnumerable<IArticleProvider> _articleProviders;

    public ShopController(
        IRepository db, 
        ICachedSupplier cachedSupplier, 
        IEnumerable<IArticleProvider> articleProviders)
    {
        _db = db;
        _cachedSupplier = cachedSupplier;
        _articleProviders = articleProviders;
    }

    [HttpGet]
    public Article? GetArticle(int id, int maxExpectedPrice = 200)
    {
        
        if (_cachedSupplier.ArticleInInventory(id))
        {
            var article = _cachedSupplier.GetArticle(id);
            if (maxExpectedPrice >= article.ArticlePrice)
            {
                return article;
            }
        }

        foreach (var articleProvider in _articleProviders)
        {
            if (!articleProvider.ArticleInInventory(id)) 
                continue;
            
            var article = articleProvider.GetArticle(id);
            if (maxExpectedPrice < article.ArticlePrice) 
                continue;
            
            _cachedSupplier.SetArticle(article);
            return article;
        }
        return null;
    }

    [HttpPost]
    public void BuyArticle(Article article, int buyerId)
    {
        if (article == null)
        {
            throw new Exception("Could not order article");
        }

        Logger.Debug($"Trying to sell article with id={article.Id}");

        article.IsSold = true;
        article.SoldDate = DateTime.Now;
        article.BuyerUserId = buyerId;

        try
        {
            _db.Save(article);
            Logger.Info($"Article with id {article.Id} is sold.");
        }
        catch (ArgumentNullException)
        {
            Logger.Error($"Could not save article with id {article.Id}");
            throw new Exception("Could not save article with id");
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
