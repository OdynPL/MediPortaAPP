using MediPorta.Domain.Entites;
using MediPorta.Infrastructure.Models;

namespace MediPorta.Application.Interfaces
{
    public interface ITagProcessor
    {
        List<Tag> Process(List<StackOverflowTag> rawTags);
    }
}
