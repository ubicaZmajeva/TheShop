using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Shop.WebApi.Commands;
using Shop.WebApi.Models;
using Shop.WebApi.Services.Repositories;
using Shop.WebApi.Services.Repositories.Entities;
using Xunit;

namespace Shop.WebApi.Tests.Commands;

public class BuyArticleCommandHandlerTests
{
    [Fact]
    public async Task Handle_ArticleNotProvided_ThrowsArgumentException()
    {
        var mockRepository = new Mock<IRepository<ArticleEntity>>();
        var mockLogger = new Mock<ILogger<BuyArticleCommandHandler>>();
        var sus = new BuyArticleCommandHandler(mockRepository.Object, mockLogger.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => sus.Handle(BuyArticleCommand.CreateInstance(null, 0), CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_ValidArticleIsProvided()
    {
        var mockRepository = new Mock<IRepository<ArticleEntity>>();

        ArticleEntity? savedArticle = null;
        mockRepository.Setup(c => c.Save(It.IsAny<ArticleEntity>()))
            .Callback<ArticleEntity>(article => savedArticle = article);
            
        var mockLogger = new Mock<ILogger<BuyArticleCommandHandler>>();
        var sus = new BuyArticleCommandHandler(mockRepository.Object, mockLogger.Object);

        var command = BuyArticleCommand.CreateInstance(new Article()
            {
                Id = new Random().Next(1, int.MaxValue),
                NameOfArticle = Guid.NewGuid().ToString("N"),
                ArticlePrice = new Random().Next(100, 500)

            },
            new Random().Next(1, int.MaxValue));

        await sus.Handle(command, CancellationToken.None);
        
        mockRepository.Verify(m => m.Save(It.IsAny<ArticleEntity>()), Times.Once);
        Assert.Equal(command.Article.Id, savedArticle?.Id);
        Assert.Equal(command.Article.NameOfArticle, savedArticle?.Name);
        Assert.Equal(command.Article.ArticlePrice, savedArticle?.Price);
        Assert.True(savedArticle?.IsSold);
        Assert.Equal(command.BuyerId, savedArticle?.BuyerUserId);
        Assert.NotNull(savedArticle?.SoldDate);
    }
    
    [Fact]
    public async Task Handle_RepositoryCannotSaveArticle_ThrowsApplicationException()
    {
        var mockRepository = new Mock<IRepository<ArticleEntity>>();

        mockRepository.Setup(c => c.Save(It.IsAny<ArticleEntity>()))
            .Throws<ApplicationException>();
            
        var mockLogger = new Mock<ILogger<BuyArticleCommandHandler>>();
        var sus = new BuyArticleCommandHandler(mockRepository.Object, mockLogger.Object);

        var command = BuyArticleCommand.CreateInstance(new Article()
            {
                Id = new Random().Next(1, int.MaxValue),
                NameOfArticle = Guid.NewGuid().ToString("N"),
                ArticlePrice = new Random().Next(100, 500)
            },
            new Random().Next(1, int.MaxValue));

        await Assert.ThrowsAsync<ApplicationException>(() => sus.Handle(command, CancellationToken.None));

    }
}