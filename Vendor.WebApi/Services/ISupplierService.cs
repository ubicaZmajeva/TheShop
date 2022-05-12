using Vendor.WebApi.Models;

namespace Vendor.WebApi.Services;

public interface ISupplierService
{
    bool ArticleInInventory(int id);
    Article GetArticle(int id);
}