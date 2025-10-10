using MediPorta.API.DTO;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class TagsControllerTests : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly HttpClient _client;

    public TagsControllerTests(WebApplicationFactoryFixture factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(DisplayName = "GET /api/tags returns paginated tags")]
    public async Task GetTags_ReturnsPaginatedTags()
    {
        var response = await _client.GetAsync("/api/tags?pageNumber=1&pageSize=10&sortBy=name&ascending=true");
        response.EnsureSuccessStatusCode();

        var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>();
        Assert.NotNull(tags);
        Assert.True(tags.Count <= 10);
    }

    [Fact(DisplayName = "POST /api/tags/refresh refreshes tags")]
    public async Task RefreshTags_EndpointWorks()
    {
        var response = await _client.PostAsync("/api/tags/refresh", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory(DisplayName = "GET /api/tags supports pagination parameters")]
    [InlineData(1, 1)]
    [InlineData(1, 2)]
    public async Task GetTags_PaginationParameters_ReturnCorrectCount(int pageNumber, int pageSize)
    {
        var response = await _client.GetAsync($"/api/tags?pageNumber={pageNumber}&pageSize={pageSize}&sortBy=name&ascending=true");
        response.EnsureSuccessStatusCode();

        var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>();
        Assert.NotNull(tags);
        Assert.True(tags.Count <= pageSize);
    }

    [Fact(DisplayName = "GET /api/tags returns tags sorted by name ascending")]
    public async Task GetTags_SortedByNameAscending_ReturnsSorted()
    {
        var response = await _client.GetAsync("/api/tags?pageNumber=1&pageSize=10&sortBy=name&ascending=true");
        response.EnsureSuccessStatusCode();

        var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>();
        Assert.NotNull(tags);

        var sorted = tags.OrderBy(t => t.Name).ToList();
        Assert.Equal(sorted.Select(t => t.Name), tags.Select(t => t.Name));
    }
}
