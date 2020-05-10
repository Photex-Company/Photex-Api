using System.Collections.Generic;
using System.Threading.Tasks;
using Photex.Core.Contracts.Models;

namespace Photex.Core.Interfaces
{
    public interface ICatalogueService
    {
        Task DeleteCatalogue(long userId, long catalogueId);
        Task AddCatalogue(long userId, string name);
        Task<IEnumerable<CatalogueSimpleModel>> GetCatalogues(long userId);
    }
}
