using cms.Core.Domain;
using cms.Core.Models;
using cms.Core.Models.Content;
using cms.Core.SeedWorks;

namespace cms.Core.Repositories
{
    public interface IPostCategoryRepository : IRepository<PostCategory, Guid>
    {
        Task<PagedResult<PostCategoryDto>> GetAllPaging(string? keyword, int pageIndex = 1, int pageSize = 10);
        Task<bool> HasPost(Guid categoryId);
        Task<PostCategoryDto> GetBySlug(string slug);
    }
}
