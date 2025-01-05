using JackOfAllCodes.Web.Models.Domain;
using JackOfAllCodes.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JackOfAllCodes.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ILikePostRepository likePostRepository;
        private readonly ICommentPostRepository commentPostRepository;
        private readonly UserManager<IdentityUser> userManager;

        public BlogsController(
            IBlogPostRepository blogPostRepository, 
            ILikePostRepository likePostRepository, 
            ICommentPostRepository commentPostRepository, 
            UserManager<IdentityUser> userManager)
        {
            this.blogPostRepository = blogPostRepository;
            this.likePostRepository = likePostRepository;
            this.commentPostRepository = commentPostRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var handle = await blogPostRepository.GetUrlHandleAsync(urlHandle);

            return View(handle);
        }

        [HttpPost]
        public async Task<IActionResult> LikePost(Guid blogPostId)
        {
            var blogPost = await blogPostRepository.GetAsync(blogPostId);
            var userId = userManager.GetUserId(User);

            if (userId != null)
            {
                var id = Guid.Parse(userId);
                var existingLike = await likePostRepository.GetAsync(blogPostId, id);

                if (existingLike != null)
                {
                    await likePostRepository.RemoveAsync(existingLike);
                }
                else
                {
                    var like = new Like
                    {
                        UserId = id,
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
                var id = Guid.Parse(userId);
                var existingLike = await likePostRepository.GetLikeCommentAsync(commentId, id);

                if (existingLike != null)
                {
                    await likePostRepository.RemoveAsync(existingLike);
                }
                else
                {
                    var like = new Like
                    {
                        UserId = id,
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
            var userId = userManager.GetUserId(User);
            var userName = userManager.GetUserName(User);

            if (userId != null)
            {
                var comment = new Comment
                {
                    Content = commentContent,
                    CreatedDate = DateTime.UtcNow,
                    BlogPostId = blogPostId,
                    UserId = Guid.Parse(userId),
                    UserName = userName,
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