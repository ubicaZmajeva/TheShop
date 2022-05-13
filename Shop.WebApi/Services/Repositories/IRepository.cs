namespace Shop.WebApi.Services.Repositories;

public interface IRepository<T>
{
    T GetById(int id);
    T Save(T article);
}