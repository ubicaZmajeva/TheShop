using Shop.WebApi.Services.Repositories.Entities;

namespace Shop.WebApi.Models;

public class Article: IHaveId
{
    public int Id { get; set; }
    public string NameOfArticle { get; set; }
    public int ArticlePrice { get; set; }
}