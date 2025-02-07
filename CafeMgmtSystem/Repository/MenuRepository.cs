using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Dapper;
using System.Data;

namespace CafeMgmtSystem.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public MenuRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        private IDbConnection Connection => _dbConnectionFactory.CreateConnection();
        public IEnumerable<MenuItem> GetAllMenuItems()
        {
            using (var connection = Connection)
            {
                connection.Open();
                return connection.Query<MenuItem>("GetAllMenuItems", commandType: CommandType.StoredProcedure);
            }
        }
        public MenuItem GetMenuItemById(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                return connection.QuerySingleOrDefault<MenuItem>(
                    "GetMenuItemById",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );
            }
        }
        public int CreateMenuItems(MenuItem menuItem)
        {
            using (var connection = Connection)
            {
                connection.Open();
                return connection.Execute(
                    "sp_CreateMenuItem",
                    new { menuItem.Name, menuItem.Price, menuItem.Category,menuItem.ImageUrl },
                    commandType: CommandType.StoredProcedure
                );
            }
        }
        public int UpdateMenuItem(MenuItem menuItem)
        {
            using (var connection = Connection)
            {
                connection.Open();
                return connection.Execute(
                    "UpdateMenuItem",
                    new { menuItem.Id, menuItem.Name, menuItem.Price, menuItem.Category },
                    commandType: CommandType.StoredProcedure
                );
            }
        }
        public int DeleteMenuItem(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                return connection.Execute(
                    "DeleteMenuItem",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );
            }
        }
    }
}
