using cms.Core.Models.Content;
using cms.Core.Models;

namespace cms.WebApp.Models
{
    public class PostListByTagViewModel
    {
        public TagDto Tag { get; set; }
        public PagedResult<PostInListDto> Posts { get; set; }
    }
}
