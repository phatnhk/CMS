using AutoMapper;
using cms.Core.Domain;

namespace cms.Core.Models.Content
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public required string Name { get; set; }

        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<Tag, TagDto>();
            }
        }
    }
}
