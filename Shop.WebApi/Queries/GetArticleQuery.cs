using MediatR;
using Shop.WebApi.Models;

namespace Shop.WebApi.Queries;

public class GetArticleQuery : IRequest<Article>
{
    private GetArticleQuery(int id, int maxExpectedPrice)
    {
        Id = id;
        MaxExpectedPrice = maxExpectedPrice;
    }

    public static GetArticleQuery CreateInstance(int id, int maxExpectedPrice)
    {
        return new GetArticleQuery(id, maxExpectedPrice);
    }

    public int Id { get; }
    public int MaxExpectedPrice { get; }
    
}