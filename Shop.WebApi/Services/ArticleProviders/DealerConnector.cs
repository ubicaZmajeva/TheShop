using Newtonsoft.Json;
using Shop.WebApi.Models;
using Shop.WebApi.Services.ArticleProviders.Core;

namespace Shop.WebApi.Services.ArticleProviders;

public class DealerConnector: IArticleProvider
{
    private readonly HttpClient _httpClient;

    public DealerConnector(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ArticleInInventory(int id) => await Get<bool>($"/Supplier/ArticleInInventory/{id}");
    public async Task<Article> GetArticle(int id) => (await Get<Response>($"/Supplier/GetArticle/{id}"))?.ToArticle();
    private async Task<T> Get<T>(string url)
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

        if (!response.IsSuccessStatusCode)
            return default;
        
        var responseContent = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseContent))
            return default;

        try
        {
            var result = JsonConvert.DeserializeObject<T>(responseContent);
            return result;
        }
        catch (Exception)
        {
            return default;
        }
    }

    public class Response
    {
        [JsonProperty(PropertyName = "ID")]
        public int Id { get; set; }
    
        [JsonProperty(PropertyName = "Name_of_article")]
        public string Name { get; set; }
    
        [JsonProperty(PropertyName = "ArticlePrice")]
        public int Price { get; set; }

        public Article ToArticle() =>
            new()
            {
                Id = Id,
                NameOfArticle = Name,
                ArticlePrice = Price
            };
    }
}