﻿using Shop.WebApi.Models;

namespace Shop.WebApi.Services.Repositories;

public class Repository: IRepository
{
    private readonly List<Article> _articles = new();

    public Article GetById(int id)
    {
        return _articles.Single(x => x.Id == id);
    }

    public void Save(Article article)
    {
        _articles.Add(article);
    }
}