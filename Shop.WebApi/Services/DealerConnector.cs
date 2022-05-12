using Newtonsoft.Json;
using Shop.WebApi.Models;

namespace Shop.WebApi.Services;

public class DealerConnector: IArticleProvider
{
    private readonly HttpClient _httpClient;

    public DealerConnector(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ArticleInInventory(int id) => await Get<bool>($"/Supplier/ArticleInInventory/{id}");
    public async Task<Article> GetArticle(int id) => await Get<Article>($"/Supplier/GetArticle/{id}");

    private async Task<T> Get<T>(string url)
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<T>(responseContent);
        return result; 
    }
}