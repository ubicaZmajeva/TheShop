using Shop.WebApi.Models;

namespace Shop.WebApi.Services;

public interface IRepository
{
    Article GetById(int id);
    void Save(Article article);
}