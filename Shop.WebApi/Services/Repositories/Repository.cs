using System.Collections.Concurrent;
using Shop.WebApi.Services.Repositories.Entities;

namespace Shop.WebApi.Services.Repositories;

public class Repository<T>: IRepository<T> where T: class, IHaveId
{
    private readonly ConcurrentDictionary<int, T> _articles = new();
    public T GetById(int id) => _articles.TryGetValue(id, out var entity) ? entity : null;
    public T Save(T entity) =>
        _articles.TryAdd(entity.Id, entity)
            ? entity
            : throw new ApplicationException($"Entity with id {entity.Id}, already exists");
}