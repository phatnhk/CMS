
using AutoMapper;
using cms.Core.Identity;
using cms.Core.Repositories;
using cms.Core.SeedWorks;
using cms.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace cms.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CMSDbContext _context;

        public UnitOfWork(CMSDbContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            Posts = new PostRepository(context, mapper, userManager);
            PostCategories = new PostCategoryRepository(context, mapper);
        }

        public IPostRepository Posts { get; private set; }
        public IPostCategoryRepository PostCategories { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
