using System;
using API.DTOs;
using API.Entities;
using API.Healper;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
           .Where(x => x.SourceUserId == currentUserId)
           .Select(x => x.TargetUserId)
           .ToListAsync();
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> qurey;

        switch (likesParams.Predicate)
        {
            case "liked":
                qurey = likes
                    .Where(x => x.SourceUserId == likesParams.userid)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                qurey = likes
                    .Where(x => x.TargetUserId == likesParams.userid)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            default:

                var likeIds = await GetCurrentUserLikeIds(likesParams.userid);

                qurey = likes
                      .Where(x => x.TargetUserId == likesParams.userid && likeIds.Contains(x.SourceUserId))
                      .Select(x => x.SourceUser)
                      .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }
        return await PagedList<MemberDto>.CreateAsync(qurey,likesParams.PageNumber,likesParams.PageSize);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
