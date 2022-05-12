using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Shop.WebApi.Models;
using Shop.WebApi.Services;
using Xunit;

namespace Shop.WebApi.Tests.Services;

public class DealerConnectorTests
{

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public async Task ArticleInInventory_ReturnProperBooleanResult(string httpResponse, bool expectedResponse)
    {
        var mockHandler = new Mock<FakeHandler> { CallBase = true };
        mockHandler
            .Setup(handler => handler.Send())
            .Returns(new HttpResponseMessage
            {
                Content = new StringContent(httpResponse)
            });
        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var sus = new DealerConnector(httpClient);

        var response = await sus.ArticleInInventory(new Random().Next(1, int.MaxValue));
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async Task GetArticle_ReturnsArticleReceivedFromOutside()
    {
        var mockHandler = new Mock<FakeHandler> { CallBase = true };
        var article = new Article
        {
            Id = new Random().Next(1, int.MaxValue)
        };
        mockHandler
            .Setup(handler => handler.Send())
            .Returns(new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(article))
            });
        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var sus = new DealerConnector(httpClient);

        var response = await sus.GetArticle(new Random().Next(1, int.MaxValue));
        
        Assert.Equal(article.Id, response.Id);
    }
    
    public abstract class FakeHandler: HttpMessageHandler
    {
        public virtual HttpResponseMessage Send() => new();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(Send());
    }
}

