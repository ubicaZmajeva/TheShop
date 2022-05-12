using System;
using Shop.WebApi.Models;
using Shop.WebApi.Services.Repositories;
using Xunit;

namespace Shop.WebApi.Tests.Services;

public class RepositoryTests
{
    [Fact]
    public void GetById_ReturnNullIfArticleIsNotFound()
    {
        var sus = new Repository();
        var result = sus.GetById(new Random().Next(0, Int32.MaxValue));
        Assert.Null(result);
    }
    
    [Fact]
    public void GetById_SaveArticleAndFetchItAgain()
    {
        var article = new Article()
        {
            Id = new Random().Next(1, Int32.MaxValue),
            NameOfArticle = Guid.NewGuid().ToString("N")
        };
        var sus = new Repository();

        sus.Save(article);
        var result = sus.GetById(article.Id);
        
        Assert.NotNull(result);
        Assert.Equal(article.Id, result.Id);
        Assert.Equal(article.NameOfArticle, result.NameOfArticle);
    }
    
    [Fact]
    public void Save_EntityWithSameIdAlreadyExists_ThrowsException()
    {
        var article = new Article
        {
            Id = new Random().Next(1, Int32.MaxValue),
            NameOfArticle = Guid.NewGuid().ToString("N")
        };
        var sus = new Repository();

        sus.Save(article);
        Assert.Throws<ApplicationException>(() => sus.Save(article));
    }
}