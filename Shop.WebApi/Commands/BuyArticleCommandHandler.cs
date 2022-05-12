using MediatR;
using Shop.WebApi.Services;

namespace Shop.WebApi.Commands;

public class BuyArticleCommandHandler : RequestHandler<BuyArticleCommand>
{
    private readonly IRepository _db;

    public BuyArticleCommandHandler(IRepository db)
    {
        _db = db;
    }
    
    protected override void Handle(BuyArticleCommand request)
    {
        if (request.Article == null)
        {
            throw new Exception("Could not order article");
        }
        
        Logger.Debug($"Trying to sell article with id={request.Article.Id}");
        
        request.Article.IsSold = true;
        request.Article.SoldDate = DateTime.Now;
        request.Article.BuyerUserId = request.BuyerId;
        
        try
        {
            _db.Save(request.Article);
            Logger.Info($"Article with id {request.Article.Id} is sold.");
        }
        catch (ArgumentNullException)
        {
            Logger.Error($"Could not save article with id {request.Article.Id}");
            throw new Exception("Could not save article with id");
        }
        catch (Exception)
        {
            // ignored
        }
    }
}