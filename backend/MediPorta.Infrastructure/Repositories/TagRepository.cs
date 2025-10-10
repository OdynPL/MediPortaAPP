using MediPorta.Domain.Entites;
using MediPorta.Infrastructure.Data;
using MediPorta.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediPorta.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllAsync() => await _context.Tags.ToListAsync();

        public async Task<List<Tag>> GetPagedAsync(int pageNumber, int pageSize, string sortBy, bool ascending)
        {
            IQueryable<Tag> query = _context.Tags;

            query = sortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name),
                "percentage" => ascending ? query.OrderBy(t => t.Percentage) : query.OrderByDescending(t => t.Percentage),
                _ => query.OrderBy(t => t.Name)
            };

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task AddOrUpdateRangeAsync(List<Tag> tags)
        {
            var existingTags = await _context.Tags.ToDictionaryAsync(t => t.Name);

            foreach (var tag in tags)
            {
                if (existingTags.TryGetValue(tag.Name, out var existing))
                {
                    existing.Count = tag.Count;
                    existing.Percentage = tag.Percentage;
                }
                else
                {
                    await _context.Tags.AddAsync(tag);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearAsync()
        {
            await _context.Tags.ExecuteDeleteAsync();
        }
    }
}
