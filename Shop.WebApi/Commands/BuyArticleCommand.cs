using MediatR;
using Shop.WebApi.Models;

namespace Shop.WebApi.Commands;

public class BuyArticleCommand: IRequest
{
    public static BuyArticleCommand CreateInstance(Article article, int buyerId)
    {
        return new BuyArticleCommand(article, buyerId);
    }

    private BuyArticleCommand(Article article, int buyerId)
    {
        Article = article;
        BuyerId = buyerId;
    }
    
    public Article Article { get; }
    public int BuyerId { get; }
   
}