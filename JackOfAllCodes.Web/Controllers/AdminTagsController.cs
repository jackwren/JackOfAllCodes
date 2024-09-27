using JackOfAllCodes.Web.Models.Domain;
using JackOfAllCodes.Web.Models.ViewModels;
using JackOfAllCodes.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace JackOfAllCodes.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            // Mapping AddTagRequest to the Tag Domain Model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tags = await tagRepository.GetAllAsync();
            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await tagRepository.GetAsync(id);

            if (data != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = data.Id,
                    Name = data.Name,
                    DisplayName = data.DisplayName
                };
                return View(editTagRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var data = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await tagRepository.UpdateAsync(data);

            if (updatedTag != null)
            {
                //Show if successful
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
            else
            {
                //Show error
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var data = await tagRepository.DeleteAsync(editTagRequest.Id);

            if (data != null)
            {
                //show a success
                return RedirectToAction("List");
            }
            else
            {
                //Show an error
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
        }
    }
}
