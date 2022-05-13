using System.Text.Json.Serialization;

namespace Vendor.WebApi.Models;

public class Article
{
    [JsonPropertyName("ID")]
    public int Id { get; set; }

    [JsonPropertyName("Name_of_article")] 
    public string NameOfArticle { get; set; } = string.Empty;
    
    public int ArticlePrice { get; set; }
    
    public bool IsSold { get; set; }
    
    public DateTime? SoldDate { get; set; }
    
    public int? BuyerUserId { get; set; }
}