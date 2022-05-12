using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Shop.WebApi.Commands;
using Shop.WebApi.Controllers;
using Shop.WebApi.Models;
using Shop.WebApi.Queries;
using Xunit;

namespace Shop.WebApi.Tests;

public class ShopControllerTests
{
    [Fact]
    public async Task GetArticle_SendsQueryToMediator()
    {
        var mockMediator = new Mock<IMediator>();
        var sus = new ShopController(mockMediator.Object);
        var articleId = new Random().Next(1, int.MaxValue);

        await sus.GetArticle(articleId);

        mockMediator.Verify(m => m.Send(
            It.Is<GetArticleQuery>(n => n.Id == articleId), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task BuyArticle_SendsCommandToMediator()
    {
        var mockMediator = new Mock<IMediator>();
        var sus = new ShopController(mockMediator.Object);
        var articleId = new Random().Next(1, int.MaxValue);
        var buyerId = new Random().Next(1, int.MaxValue);

        await sus.BuyArticle(
            new Article
            {
                Id = articleId
            }, buyerId, CancellationToken.None);

        mockMediator.Verify(m => m.Send(
            It.Is<BuyArticleCommand>(
                n => n.Article.Id == articleId 
                && n.BuyerId == buyerId
                ), 
            CancellationToken.None), 
            Times.Once);
    }
}