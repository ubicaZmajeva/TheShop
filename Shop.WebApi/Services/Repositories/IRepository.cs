using Shop.WebApi.Models;

namespace Shop.WebApi.Services.Repositories;

public interface IRepository
{
    Article GetById(int id);
    Article Save(Article article);
}