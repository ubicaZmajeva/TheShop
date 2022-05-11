using Moq;
using Shop.WebApi.Controllers;
using Shop.WebApi.Models;
using Shop.WebApi.Services;
using Xunit;

namespace Shop.WebApi.Tests;

public class ShopControllerTests
{
    private readonly Mock<IRepository> _mockRepository = new();
    private readonly Mock<ICachedSupplier> _mockCachedSupplier = new ();
    private readonly Mock<IArticleProvider> _mockWarehouse = new ();
    private readonly Mock<IArticleProvider> _mockDealer1 = new ();
    private readonly Mock<IArticleProvider> _mockDealer2 = new ();
    
    [Fact]
    public void GetArticle_IfArticleExistsInCacheUseItIfPriceIsRight()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 100
        };
        
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => true);
        _mockCachedSupplier.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => dummyArticle);
        
        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = sus.GetArticle(0);
        
        Assert.NotNull(result);
        Assert.Equal(dummyArticle, result);
    }
    
    [Fact]
    public void GetArticle_IfArticleExistsInCacheButPriceIsTooHighProbeWarehouse()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 210
        };
        
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => true);
        _mockCachedSupplier.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => dummyArticle);
        
        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        sus.GetArticle(0);
        _mockWarehouse.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public void GetArticle_IfArticleDoesNotExistsInCacheProbeWarehouse()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        
        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        sus.GetArticle(0);
        _mockWarehouse.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public void GetArticle_UseArticleFromWareHouseIfPriceIsRight()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 100
        };
        
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => true);
        _mockWarehouse.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => dummyArticle);
        
        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = sus.GetArticle(0);
        
        Assert.NotNull(result);
        Assert.Equal(dummyArticle, result);
    }
    
    [Fact]
    public void GetArticle_IfNoArticleInCacheOrWarehouseProbeDealer1()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        
        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        sus.GetArticle(0);
        _mockDealer1.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public void GetArticle_IfNoArticleInCacheOrWarehouseOrDealer1ProbeDealer2()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);

        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        sus.GetArticle(0);
        _mockDealer2.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public void GetArticle_ReturnArticleFromDealer2IfPriceIsRight()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 120
        };

        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockDealer2.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => true);
        _mockDealer2.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => dummyArticle);

        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = sus.GetArticle(0);
        Assert.NotNull(result);
        Assert.Equal(dummyArticle, result);
    }
    
    [Fact]
    public void GetArticle_ReturnNullIfArticleDealer2IsTooExpensive()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 220
        };

        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockDealer2.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => true);
        _mockDealer2.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => dummyArticle);

        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = sus.GetArticle(0);
        Assert.Null(result);
    }
    
    [Fact]
    public void GetArticle_ReturnNullIfArticleCannotBeFound()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockDealer2.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);

        var sus = new ShopController(
            _mockRepository.Object,
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = sus.GetArticle(0);
        Assert.Null(result);
    }
}