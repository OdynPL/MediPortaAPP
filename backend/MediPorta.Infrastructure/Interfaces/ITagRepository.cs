using MediPorta.Domain.Entites;

namespace MediPorta.Infrastructure.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();
        Task<List<Tag>> GetPagedAsync(int pageNumber, int pageSize, string sortBy, bool ascending);
        Task AddOrUpdateRangeAsync(List<Tag> tags);
        Task ClearAsync();
    }
}
