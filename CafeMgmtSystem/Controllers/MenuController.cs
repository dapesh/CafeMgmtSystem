using CafeMgmtSystem.Models;
using CafeMgmtSystem.Repository;
using CafeMgmtSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeMgmtSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuItemRepository;
        private readonly CloudinaryService _cloudinaryService;
        public MenuController(IMenuRepository menuItemRepository, CloudinaryService cloudinaryService)
        {
            _menuItemRepository = menuItemRepository;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllMenuItems()
        {
            var menuItems = _menuItemRepository.GetAllMenuItems();
            return Ok(menuItems);
        }

        [HttpGet("{id}")]
        public IActionResult GetMenuItemById(int id)
        {
            var menuItem = _menuItemRepository.GetMenuItemById(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return Ok(menuItem);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateMenuItem([FromForm] MenuItem menuItem, [FromForm] IFormFile imageFile)
        //{
        //    if (imageFile == null || imageFile.Length == 0)
        //    {
        //        return BadRequest("Image file is required.");
        //    }
        //    var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile);
        //    if (string.IsNullOrEmpty(imageUrl))
        //    {
        //        return BadRequest("Image upload failed.");
        //    }
        //    menuItem.ImageUrl = imageUrl;
        //    var result = _menuItemRepository.CreateMenuItems(menuItem);
        //    if (result == 0)
        //    {
        //        return BadRequest("Failed to create menu item.");
        //    }
        //    return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItem.Id }, menuItem);
        //}

        [HttpPut("{id}")]
        public IActionResult UpdateMenuItem(int id, [FromBody] MenuItem menuItem)
        {
            menuItem.Id = id;
            var result = _menuItemRepository.UpdateMenuItem(menuItem);
            if (result == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMenuItem(int id)
        {
            var result = _menuItemRepository.DeleteMenuItem(id);
            if (result == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
