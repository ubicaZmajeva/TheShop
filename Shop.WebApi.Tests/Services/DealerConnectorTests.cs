using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Shop.WebApi.Services.ArticleProviders;
using Xunit;

namespace Shop.WebApi.Tests.Services;

public class DealerConnectorTests
{
    private Mock<ILogger<DealerConnector>> mockLogger = new();

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
        var sus = new DealerConnector(httpClient, mockLogger.Object);

        var response = await sus.ArticleInInventory(new Random().Next(1, int.MaxValue));
        Assert.Equal(expectedResponse, response);
    }

    [Fact]
    public async Task GetArticle_ReturnsArticleReceivedFromOutside()
    {
        var mockHandler = new Mock<FakeHandler> { CallBase = true };
        mockHandler
            .Setup(handler => handler.Send())
            .Returns(new HttpResponseMessage
            {
                Content = new StringContent("{\"ID\":123, \"Name_of_article\":\"Test\", \"ArticlePrice\":100}")
            });
        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        var sus = new DealerConnector(httpClient, mockLogger.Object);

        var response = await sus.GetArticle(new Random().Next(1, int.MaxValue));
        
        Assert.Equal(123, response.Id);
    }
    
    [ExcludeFromCodeCoverage]
    public abstract class FakeHandler: HttpMessageHandler
    {
        public virtual HttpResponseMessage Send() => new();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(Send());
    }
}

