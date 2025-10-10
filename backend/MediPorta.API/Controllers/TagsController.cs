using MediPorta.API.DTO;
using MediPorta.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediPorta.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: api/tags?pageNumber=1&pageSize=50&sortBy=name&ascending=true
        [HttpGet]
        public async Task<ActionResult<List<TagDto>>> GetTags(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string sortBy = "name",
            [FromQuery] bool ascending = true)
        {
            var tags = await _tagService.GetTagsAsync(pageNumber, pageSize, sortBy, ascending);
            return Ok(tags);
        }

        // POST: api/tags/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTags()
        {
            try
            {
                await _tagService.RefreshTagsFromStackOverflowAsync();
                return Ok(new { message = "Tags refreshed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
