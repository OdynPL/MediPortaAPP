using AutoMapper;
using MediPorta.API.DTO;
using MediPorta.Application.Interfaces;
using MediPorta.Infrastructure.Interfaces;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;
    private readonly IStackOverflowClient _stackClient;
    private readonly ITagProcessor _processor;
    private readonly IMapper _mapper;

    public TagService(
        ITagRepository repository,
        IStackOverflowClient stackClient,
        ITagProcessor processor,
        IMapper mapper)
    {
        _repository = repository;
        _stackClient = stackClient;
        _processor = processor;
        _mapper = mapper;
    }

    public async Task<List<TagDto>> GetTagsAsync(int pageNumber, int pageSize, string sortBy, bool ascending)
    {
        var tags = await _repository.GetPagedAsync(pageNumber, pageSize, sortBy, ascending);
        return _mapper.Map<List<TagDto>>(tags);
    }

    public async Task RefreshTagsFromStackOverflowAsync()
    {
        var rawTags = await _stackClient.GetTagsAsync(1000);
        var tags = _processor.Process(rawTags);

        await _repository.ClearAsync();
        await _repository.AddOrUpdateRangeAsync(tags);
    }
}
