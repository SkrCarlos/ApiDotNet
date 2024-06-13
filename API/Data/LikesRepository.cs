using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int TargetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, TargetUserId);
    }

    public async Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams Lp)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if (Lp.Predicate.ToLowerInvariant() == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == Lp.UserId);
            users = likes.Select(like => like.TargetUser);
        }

        if (Lp.Predicate.ToLowerInvariant() == "likedBy")
        {
            likes = likes.Where(like => like.TargetUserId == Lp.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        var likedUsers = users.Select(user => new LikeDto
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDto>.CreateAsync(likedUsers, Lp.PageNumber, Lp.PageSize);
    }

    public async Task<AppUser> GetUserWithLikesAsync(int userId)
    {
        return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}