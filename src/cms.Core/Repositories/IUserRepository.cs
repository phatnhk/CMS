using cms.Core.Identity;
using cms.Core.SeedWorks;

namespace cms.Core.Repositories
{
    public interface IUserRepository : IRepository<AppUser, Guid>
    {
        Task RemoveUserFromRolesByAsync(Guid userId, string[] roles);
    }
}
