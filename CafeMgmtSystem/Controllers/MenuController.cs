using CafeMgmtSystem.Models;
using CafeMgmtSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeMgmtSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuItemRepository;

        public MenuController(IMenuRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        [HttpGet]
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

        [HttpPost]
        public IActionResult CreateMenuItem([FromBody] MenuItem menuItem)
        {
            var result = _menuItemRepository.CreateMenuItem(menuItem);
            if (result == 0)
            {
                return BadRequest("Failed to create menu item.");
            }
            return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItem.Id }, menuItem);
        }

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
