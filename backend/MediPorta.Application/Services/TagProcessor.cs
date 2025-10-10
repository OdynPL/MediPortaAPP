using MediPorta.Application.Interfaces;
using MediPorta.Domain.Entites;
using MediPorta.Infrastructure.Models;

namespace MediPorta.Application.Services
{
    public class TagProcessor : ITagProcessor
    {
        public List<Tag> Process(List<StackOverflowTag> rawTags)
        {
            var tags = rawTags.Select(t => new Tag
            {
                Name = t.Name,
                Count = t.Count
            }).ToList();

            var totalCount = tags.Sum(t => t.Count);

            if (totalCount == 0) 
                return tags;

            foreach (var tag in tags)
                tag.Percentage = Math.Round((decimal)tag.Count / totalCount * 100, 4);

            return tags;
        }
    }
}
