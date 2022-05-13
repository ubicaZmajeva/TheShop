using System;
using Shop.WebApi.Services.Repositories;
using Shop.WebApi.Services.Repositories.Entities;
using Xunit;

namespace Shop.WebApi.Tests.Services;

public class RepositoryTests
{
    [Fact]
    public void GetById_ReturnNullIfArticleIsNotFound()
    {
        var sus = new Repository<ArticleEntity>();
        var result = sus.GetById(new Random().Next(0, Int32.MaxValue));
        Assert.Null(result);
    }
    
    [Fact]
    public void GetById_SaveArticleAndFetchItAgain()
    {
        var articleEntity = new ArticleEntity()
        {
            Id = new Random().Next(1, Int32.MaxValue),
            Name = Guid.NewGuid().ToString("N")
        };
        var sus = new Repository<ArticleEntity>();

        sus.Save(articleEntity);
        var result = sus.GetById(articleEntity.Id);
        
        Assert.NotNull(result);
        Assert.Equal(articleEntity.Id, result.Id);
        Assert.Equal(articleEntity.Name, result.Name);
    }
    
    [Fact]
    public void Save_EntityWithSameIdAlreadyExists_ThrowsException()
    {
        var articleEntity = new ArticleEntity
        {
            Id = new Random().Next(1, Int32.MaxValue),
            Name = Guid.NewGuid().ToString("N")
        };
        var sus = new Repository<ArticleEntity>();

        sus.Save(articleEntity);
        Assert.Throws<ApplicationException>(() => sus.Save(articleEntity));
    }
}