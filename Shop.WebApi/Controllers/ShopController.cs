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
    private readonly IWarehouse _warehouse;
    private readonly IDealer1 _dealer1;
    private readonly IDealer2 _dealer2;

    public ShopController(
        IRepository db, 
        ICachedSupplier cachedSupplier, 
        IWarehouse warehouse, 
        IDealer1 dealer1, 
        IDealer2 dealer2)
    {
        _db = db;
        _cachedSupplier = cachedSupplier;
        _warehouse = warehouse;
        _dealer1 = dealer1;
        _dealer2 = dealer2;
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
        
        if (_warehouse.ArticleInInventory(id))
        {
            var article = _warehouse.GetArticle(id);
            if (maxExpectedPrice >= article.ArticlePrice)
            {
                _cachedSupplier.SetArticle(article);
                return article;
            }
        }
        
        if (_dealer1.ArticleInInventory(id))
        {
            var article = _dealer1.GetArticle(id);
            if (maxExpectedPrice >= article.ArticlePrice)
            {
                _cachedSupplier.SetArticle(article);
                return article;
            }
        }
        
        if (_dealer2.ArticleInInventory(id))
        {
            var article = _dealer2.GetArticle(id);
            if (maxExpectedPrice >= article.ArticlePrice)
            {
                _cachedSupplier.SetArticle(article);
                return article;
            }
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
