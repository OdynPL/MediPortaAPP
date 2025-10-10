using MediPorta.API.DTO;

namespace MediPorta.Application.Interfaces
{
    public interface ITagService
    {
        Task<List<TagDto>> GetTagsAsync(int pageNumber, int pageSize, string sortBy, bool ascending);
        Task RefreshTagsFromStackOverflowAsync();
    }
}
