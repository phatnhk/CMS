using cms.Core.Domain;
using cms.Core.Models.Content;
using cms.Core.SeedWorks;

namespace cms.Core.Repositories
{
    public interface ITagRepository : IRepository<Tag, Guid>
    {
        Task<TagDto> GetBySlug(string slug);
    }
}
