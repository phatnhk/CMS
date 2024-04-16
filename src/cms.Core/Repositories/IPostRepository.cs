using cms.Core.Domain;
using cms.Core.Models;
using cms.Core.Models.Content;
using cms.Core.SeedWorks;

namespace cms.Core.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<PagedResult<PostInListDto>> GetAllPaging(string? keyword, Guid currentUserId, Guid? categoryId, int pageIndex = 1, int pageSize = 10);
        Task<bool> IsSlugAlreadyExisted(string slug, Guid? currentId = null);
        Task Approve(Guid id, Guid currentUserId);
        Task SendToApprove(Guid id, Guid currentUserId);
        Task ReturnBack(Guid id, Guid currentUserId, string note);
        Task<string> GetReturnReason(Guid id);
        Task<bool> HasPublishInLast(Guid id);
        Task<List<Post>> GetListUnpaidPublishPosts(Guid userId);
        Task<PostDto> GetBySlug(string slug);
        Task<List<string>> GetAllTags();
        Task AddTagToPost(Guid postId, Guid tagId);
        Task<List<string>> GetTagsByPostId(Guid postId);
    }
}
