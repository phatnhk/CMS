using cms.Core.Models.Content;

namespace cms.WebApp.Models
{
    public class PostDetailViewModel
    {
        public PostDto Post { get; set; }
        public PostCategoryDto Category { get; set; }
    }
}
