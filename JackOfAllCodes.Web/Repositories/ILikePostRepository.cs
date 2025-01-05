﻿
using JackOfAllCodes.Web.Models.Domain;

namespace JackOfAllCodes.Web.Repositories
{
    public interface ILikePostRepository
    {
        Task AddAsync(Like like);
        Task<Like?> GetAsync(Guid blogPostId, Guid userId);
        Task RemoveAsync(Like existingLike);
        Task<Like?> GetLikeCommentAsync(Guid commentId, Guid userId);
    }
}