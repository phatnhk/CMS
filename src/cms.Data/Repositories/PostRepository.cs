﻿using AutoMapper;
using cms.Core.Domain;
using cms.Core.Identity;
using cms.Core.Models;
using cms.Core.Models.Content;
using cms.Core.Repositories;
using cms.Core.SeedWorks.Constants;
using cms.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace cms.Data.Repositories
{
    public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public PostRepository(CMSDbContext context, IMapper mapper, UserManager<AppUser> userManager) : base(context)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<PagedResult<PostInListDto>> GetAllPaging(string? keyword, Guid currentUserId, Guid? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                throw new Exception("Không tồn tại user");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var canApprove = false;
            if (roles.Contains(Roles.Admin))
            {
                canApprove = true;
            }
            else
            {
                canApprove = await _context.RoleClaims.AnyAsync(x => roles.Contains(x.RoleId.ToString())
                           && x.ClaimValue == Permissions.Posts.Approve);
            }

            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (!canApprove)
            {
                query = query.Where(x => x.AuthorUserId == currentUserId);
            }

            var totalRow = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated)
               .Skip((pageIndex - 1) * pageSize)
               .Take(pageSize);

            return new PagedResult<PostInListDto>
            {
                Results = await _mapper.ProjectTo<PostInListDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };
        }

        public async Task AddTagToPost(Guid postId, Guid tagId)
        {
            await _context.PostTags.AddAsync(new PostTag()
            {
                PostId = postId,
                TagId = tagId
            });
        }

        public async Task Approve(Guid id, Guid currentUserId)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                throw new Exception("Không tồn tại bài viết");
            }
            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null)
            {
                throw new Exception("Không tồn tại user");
            }
            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                Id = Guid.NewGuid(),
                FromStatus = post.Status,
                ToStatus = PostStatus.Published,
                UserId = currentUserId,
                UserName = user.UserName,
                PostId = id,
                Note = $"{user?.UserName} duyệt bài"
            });
            post.Status = PostStatus.Published;
            _context.Posts.Update(post);
        }

        public async Task<List<string>> GetAllTags()
        {
            var query = _context.Tags.Select(x => x.Name);

            return await query.ToListAsync();
        }

        public async Task<PostDto> GetBySlug(string slug)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Slug == slug);
            if (post == null) throw new Exception($"Cannot find post with Slug: {slug}");
            return _mapper.Map<PostDto>(post);
        }

        public async Task<List<Post>> GetListUnpaidPublishPosts(Guid userId)
        {
            return await _context.Posts
               .Where(x => x.AuthorUserId == userId && x.IsPaid == false
                       && x.Status == PostStatus.Published)
               .ToListAsync();
        }

        public async Task<string> GetReturnReason(Guid id)
        {
            var activity = await _context.PostActivityLogs
                .Where(x => x.PostId == id && x.ToStatus == PostStatus.Rejected)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefaultAsync();
            return activity?.Note;
        }

        public async Task<List<string>> GetTagsByPostId(Guid postId)
        {
            var query = from post in _context.Posts
                        join pt in _context.PostTags on post.Id equals pt.PostId
                        join t in _context.Tags on pt.TagId equals t.Id
                        where post.Id == postId
                        select t.Name;
            return await query.ToListAsync();
        }

        public async Task<bool> HasPublishInLast(Guid id)
        {
            var hasPublished =
                await _context.PostActivityLogs.CountAsync(x => x.PostId == id
                && x.ToStatus == PostStatus.Published);
            return hasPublished > 0;
        }

        public async Task<bool> IsSlugAlreadyExisted(string slug, Guid? currentId = null)
        {
            if (currentId.HasValue)
            {
                return await _context.Posts.AnyAsync(x => x.Slug == slug && x.Id != currentId.Value);
            }
            return await _context.Posts.AnyAsync(x => x.Slug == slug);
        }

        public async Task ReturnBack(Guid id, Guid currentUserId, string note)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                throw new Exception("Không tồn tại bài viết");
            }

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                throw new Exception("Không tồn tại user");
            }

            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                FromStatus = post.Status,
                ToStatus = PostStatus.Rejected,
                UserId = currentUserId,
                UserName = user.UserName,
                PostId = post.Id,
                Note = note
            });

            post.Status = PostStatus.Rejected;
            _context.Posts.Update(post);
        }

        public async Task SendToApprove(Guid id, Guid currentUserId)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                throw new Exception("Không tồn tại bài viết");
            }
            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                throw new Exception("Không tồn tại user");
            }
            await _context.PostActivityLogs.AddAsync(new PostActivityLog
            {
                FromStatus = post.Status,
                ToStatus = PostStatus.WaitingForApproval,
                UserId = currentUserId,
                PostId = post.Id,
                UserName = user.UserName,
                Note = $"{user.UserName} gửi bài chờ duyệt"
            });

            post.Status = PostStatus.WaitingForApproval;
            _context.Posts.Update(post);
        }
    }
}
