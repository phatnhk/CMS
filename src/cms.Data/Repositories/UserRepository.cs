using cms.Core.Identity;
using cms.Core.Repositories;
using cms.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace cms.Data.Repositories
{
    public class UserRepository : RepositoryBase<AppUser, Guid>, IUserRepository
    {
        public UserRepository(CMSDbContext context) : base(context)
        {
        }

        public async Task RemoveUserFromRolesByAsync(Guid userId, string[] roles)
        {
            if (roles == null || roles.Length == 0)
                return;
            foreach (var roleName in roles)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
                if (role == null)
                {
                    return;
                }
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.RoleId == role.Id && x.UserId == userId);
                if (userRole == null)
                {
                    return;
                }
                _context.UserRoles.Remove(userRole);
            }
        }
    }
}
