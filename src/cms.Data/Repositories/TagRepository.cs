using AutoMapper;
using cms.Core.Domain;
using cms.Core.Models.Content;
using cms.Core.Repositories;
using cms.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace cms.Data.Repositories
{
    public class TagRepository : RepositoryBase<Tag, Guid>, ITagRepository
    {
        private readonly IMapper _mapper;
        public TagRepository(CMSDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<TagDto?> GetBySlug(string slug)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(x => x.Slug == slug);
            if (tag == null) return null;
            return _mapper.Map<TagDto?>(tag);
        }
    }
}
