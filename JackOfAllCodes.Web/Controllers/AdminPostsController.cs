using JackOfAllCodes.Web.Models.Domain;
using JackOfAllCodes.Web.Models.ViewModels;
using JackOfAllCodes.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JackOfAllCodes.Web.Controllers
{
    public class AdminPostsController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRepository blogPostRepository;

        public AdminPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Call the repository get all tags from DB into tags
            var tags = await tagRepository.GetAllAsync();

            //Build model and add all tag into List
            var model = new AddPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPostRequest addPostRequest)
        {
            //Map view model to Domain model
            var blogPost = new BlogPost
            {
                Heading = addPostRequest.Heading,
                PageTitle = addPostRequest.PageTitle,
                Content = addPostRequest.Content,
                ShortDescription = addPostRequest.ShortDescription,
                FeaturedImageUrl = addPostRequest.FeaturedImageUrl,
                UrlHandle = addPostRequest.UrlHandle,
                PublishedDate = addPostRequest.PublishedDate.ToUniversalTime(),
                Author = addPostRequest.Author,
                Visible = addPostRequest.Visible
                //Does not include tags selected. Have mapped those below
            };

            //Map tags from selected tags
            var selectedTags = new List<Tag>();
            foreach (var tagId in addPostRequest.SelectedTags)
            {
                var selectedTagId = Guid.Parse(tagId);
                var exisitingTag = await tagRepository.GetAsync(selectedTagId);

                if (exisitingTag != null)
                {
                    selectedTags.Add(exisitingTag);
                }
            }

            //Mapping tags back to domain model
            blogPost.Tag = selectedTags;

            await blogPostRepository.AddAsync(blogPost);

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {

            //Call the repository to gather Data from DB
            var blogPosts = await blogPostRepository.GetAllAsync();

            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //Return result from repository
            var blogPost = await blogPostRepository.GetAsync(id);
            var tags = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {
                //Map domain into view model
                var editBlogRequest = new EditPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    ShortDescription = blogPost.ShortDescription,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    Tags = tags.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tag.Select(x => x.Id.ToString()).ToArray()
                };

                return View(editBlogRequest);

            }

            return View(null);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostRequest editBlogPostRequest)
        {
            //Map view model to domain model
            var model = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                UrlHandle = editBlogPostRequest.UrlHandle,
                PublishedDate = editBlogPostRequest.PublishedDate,
                Author = editBlogPostRequest.Author,
                Visible = editBlogPostRequest.Visible
            };

            //Map tags into model
            var selectedTags = new List<Tag>();

            foreach (var item in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(item, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            model.Tag = selectedTags;

            //Append data to database via interface repo
            var updatedBlog = await blogPostRepository.UpdateAsync(model);

            if (updatedBlog != null)
            {
                //Show success notification
                return RedirectToAction("List");
            }

            //show error
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditPostRequest editBlogPostRequest)
        {
            //Use interface to delete this model
            var deletedPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedPost != null)
            {
                //show success
                return RedirectToAction("List");
            }
            //show error
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }
    }
}
