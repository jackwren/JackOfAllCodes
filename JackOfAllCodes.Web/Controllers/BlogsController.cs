using JackOfAllCodes.Web.Models.Domain;
using JackOfAllCodes.Web.Models.ViewModels;
using JackOfAllCodes.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JackOfAllCodes.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ILikePostRepository likePostRepository;
        private readonly ICommentPostRepository commentPostRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public BlogsController(
            IBlogPostRepository blogPostRepository, 
            ILikePostRepository likePostRepository, 
            ICommentPostRepository commentPostRepository, 
            UserManager<ApplicationUser> userManager)
        {
            this.blogPostRepository = blogPostRepository;
            this.likePostRepository = likePostRepository;
            this.commentPostRepository = commentPostRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var blogPost = await blogPostRepository.GetUrlHandleAsync(urlHandle);

            if (blogPost == null)
            {
                return NotFound();
            }

            // Get distinct users who have commented on the blog post
            var userIds = blogPost.Comments.Select(comment => comment.UserId).Distinct();
            var users = await userManager.Users
                .Where(user => userIds.Contains(user.Id))
                .ToListAsync();

            var blogsView = new BlogsViewModel
            {
                BlogPost = blogPost,
                Users = users
            };

            return View(blogsView);
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(Guid blogPostId)
        {
            var blogPost = await blogPostRepository.GetAsync(blogPostId);
            var userId = userManager.GetUserId(User);

            if (userId != null)
            {
                var existingLike = await likePostRepository.GetAsync(blogPostId, userId);

                if (existingLike != null)
                {
                    await likePostRepository.RemoveAsync(existingLike);
                }
                else
                {
                    var like = new Like
                    {
                        UserId = userId,
                        BlogPostId = blogPostId
                    };
                    await likePostRepository.AddAsync(like);
                }
            }

            return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
        }

        [HttpPost]
        public async Task<IActionResult> LikeComment(Guid blogPostId, Guid commentId)
        {
            var blogPost = await blogPostRepository.GetAsync(blogPostId);
            var userId = userManager.GetUserId(User);

            if (userId != null)
            {
                var existingLike = await likePostRepository.GetLikeCommentAsync(commentId, userId);

                if (existingLike != null)
                {
                    await likePostRepository.RemoveAsync(existingLike);
                }
                else
                {
                    var like = new Like
                    {
                        UserId = userId,
                        CommentId = commentId
                    };
                    await likePostRepository.AddAsync(like);
                }
            }

            return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Guid blogPostId, string commentContent)
        {
            var blogPost = await blogPostRepository.GetAsync(blogPostId);
            var user = await userManager.GetUserAsync(User);

            if (user != null && user.UserName != null)
            {
                var comment = new Comment
                {
                    Content = commentContent,
                    CreatedDate = DateTime.UtcNow,
                    BlogPostId = blogPostId,
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsVisible = true
                };

                await commentPostRepository.AddAsync(comment);
            }

            return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid blogPostId, Guid commentId)
        {
            var comment = await commentPostRepository.GetAsync(blogPostId, commentId);
            var userId = userManager.GetUserId(User);

            if (userId != null && comment != null)
            {
                await commentPostRepository.DeleteAsync(comment);          
            }

            var blogPost = await blogPostRepository.GetAsync(blogPostId);
            return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
        }
    }
}