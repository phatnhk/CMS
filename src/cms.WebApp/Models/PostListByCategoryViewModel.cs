using cms.Core.Models.Content;
using cms.Core.Models;

namespace cms.WebApp.Models
{
    public class PostListByCategoryViewModel
    {
        public PostCategoryDto Category { get; set; }
        public PagedResult<PostInListDto> Posts { get; set; }
    }
}
