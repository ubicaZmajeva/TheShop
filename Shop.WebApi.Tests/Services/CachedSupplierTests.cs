using System;
using Moq;
using Shop.WebApi.Models;
using Shop.WebApi.Services.Cache;
using Shop.WebApi.Services.Repositories;
using Xunit;

namespace Shop.WebApi.Tests.Services;

public class CachedSupplierTests
{
    [Fact]
    public void ArticleInInventory_IfInnerRepositoryDoesNotContainItemReturnFalse()
    {
        var mockRepository = new Mock<IRepository>();

        mockRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns(() => null!);
        var sus = new CachedSupplier(mockRepository.Object);
        
        var result = sus.ArticleInInventory(new Random().Next(1, Int32.MaxValue));
        
        Assert.False(result);
    }
    
    [Fact]
    public void ArticleInInventory_IfInnerRepositoryContainItemReturnTrue()
    {
        var mockRepository = new Mock<IRepository>();

        mockRepository.Setup(m => m.GetById(It.IsAny<int>())).Returns(() => new Article());
        var sus = new CachedSupplier(mockRepository.Object);
        
        var result = sus.ArticleInInventory(new Random().Next(1, Int32.MaxValue));
        
        Assert.True(result);
    }


    [Fact]
    public void SetArticle_SaveToCacheAndRetrieve()
    {
        var article = new Article()
        {
            Id = new Random().Next(1, Int32.MaxValue),
            NameOfArticle = Guid.NewGuid().ToString("N")
        };
        
        var sus = new CachedSupplier(new Repository());

        sus.SetArticle(article);
        var result = sus.GetArticle(article.Id);
        
        Assert.NotNull(result);
        Assert.Equal(article.Id, result.Id);
        Assert.Equal(article.NameOfArticle, result.NameOfArticle);

    }
    
}