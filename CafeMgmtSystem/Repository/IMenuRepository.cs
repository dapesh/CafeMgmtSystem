using CafeMgmtSystem.Models;

namespace CafeMgmtSystem.Repository
{
    public interface IMenuRepository
    {
        IEnumerable<MenuItem> GetAllMenuItems();
        MenuItem GetMenuItemById(int id);
        int CreateMenuItems(MenuItem menuItem);
        int UpdateMenuItem(MenuItem menuItem);
        int DeleteMenuItem(int id);
    }
}
