using JackOfAllCodes.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace JackOfAllCodes.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;

        public BlogsController(IBlogPostRepository blogPostRepository)
        {
            this.blogPostRepository = blogPostRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var handle = await blogPostRepository.GetUrlHandleAsync(urlHandle);

            return View(handle);
        }
    }
}
