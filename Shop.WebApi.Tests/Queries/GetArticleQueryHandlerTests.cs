using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shop.WebApi.Models;
using Shop.WebApi.Queries;
using Shop.WebApi.Services.ArticleProviders.Core;
using Shop.WebApi.Services.Cache;
using Xunit;

namespace Shop.WebApi.Tests.Queries;

public class GetArticleQueryHandlerTests {
    
    private readonly Mock<ICachedSupplier> _mockCachedSupplier = new ();
    private readonly Mock<IArticleProvider> _mockWarehouse = new ();
    private readonly Mock<IArticleProvider> _mockDealer1 = new ();
    private readonly Mock<IArticleProvider> _mockDealer2 = new ();
    
    [Fact]
    public async Task Handle_IfArticleExistsInCacheUseItIfPriceIsRight()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 100
        };
        
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => true);
        _mockCachedSupplier.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => dummyArticle);
        
        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(dummyArticle, result);
    }
    
    [Fact]
    public async Task Handle_IfArticleExistsInCacheButPriceIsTooHighProbeWarehouse()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 210
        };
        
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => true);
        _mockCachedSupplier.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => dummyArticle);
        
        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        _mockWarehouse.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public async Task Handle_IfArticleDoesNotExistsInCacheProbeWarehouse()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        
        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        _mockWarehouse.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public async Task Handle_UseArticleFromWareHouseIfPriceIsRight()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 100
        };
        
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(true));
        _mockWarehouse.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(dummyArticle));
        
        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(dummyArticle, result);
    }
    
    [Fact]
    public async Task Handle_IfNoArticleInCacheOrWarehouseProbeDealer1()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        
        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        _mockDealer1.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public async Task Handle_IfNoArticleInCacheOrWarehouseOrDealer1ProbeDealer2()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));

        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        _mockDealer2.Verify(m => m.ArticleInInventory(0), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ReturnArticleFromDealer2IfPriceIsRight()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 120
        };

        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        _mockDealer2.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(true));
        _mockDealer2.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(dummyArticle));

        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(dummyArticle, result);
    }
    
    [Fact]
    public async Task Handle_ReturnNullIfArticleDealer2IsTooExpensive()
    {
        var dummyArticle = new Article
        {
            ArticlePrice = 220
        };

        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        _mockDealer2.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(true));
        _mockDealer2.Setup(m => m.GetArticle(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(dummyArticle));

        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task Handle_ReturnNullIfArticleCannotBeFound()
    {
        _mockCachedSupplier.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => false);
        _mockWarehouse.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        _mockDealer1.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));
        _mockDealer2.Setup(m => m.ArticleInInventory(It.IsAny<int>())).Returns<int>(_ => Task.FromResult(false));

        var sus = new GetArticleQueryHandler(
            _mockCachedSupplier.Object,
            new[]
            {
                _mockWarehouse.Object,
                _mockDealer1.Object,
                _mockDealer2.Object 
            }
        );
        
        var result = await sus.Handle(GetArticleQuery.CreateInstance(0, 200), CancellationToken.None);
        Assert.Null(result);
    }
}