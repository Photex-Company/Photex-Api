using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Photex.Core.Contracts.Models;
using Photex.Core.Interfaces;
using Photex.Database;
using Photex.Database.Entities;

namespace Photex.Core.Services
{
    public class CatalogueService : ICatalogueService
    {
        private readonly PhotexDbContext _context;

        public CatalogueService(PhotexDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CatalogueSimpleModel>> GetCatalogues(long userId)
        {
            var catalogues = await _context.Catalogues
                .Where(x => x.UserId == userId)
                .Include(x => x.Images)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Images.FirstOrDefault(),
                    LastModified = x.Images.Max(i => i.DateModified)
                })
                .ToListAsync();

            return catalogues.Select(x => new CatalogueSimpleModel
            {
                Id = x.Id,
                Name = x.Name,
                CoverImageUrl = x.Image?.Url,
                LastModified = x.LastModified
            });
        }

        public async Task AddCatalogue(long userId, string name)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var catalogue = await _context.Catalogues
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x =>
                    x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    x.UserId == userId);

                if (catalogue == null)
                {

                    catalogue = new Catalogue
                    {
                        Name = name,
                        Images = new List<Image>(),
                        UserId = userId
                    };

                    await _context.Catalogues.AddAsync(catalogue);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
            }
        }

        public async Task DeleteCatalogue(long userId, long catalogueId)
        {
            var catalogue = await _context.Catalogues
                .Where(x => x.UserId == userId && x.Id == catalogueId)
                .FirstOrDefaultAsync();

            if (catalogue != null)
            {
                _context.Remove(catalogue);
                await _context.SaveChangesAsync();
            }
        }
    }
}
