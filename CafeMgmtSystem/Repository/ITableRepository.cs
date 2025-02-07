using CafeMgmtSystem.Models;

namespace CafeMgmtSystem.Repository
{
    public interface ITableRepository
    {
        IEnumerable<Table> GetAllTables();
        Table GetTableById(int id);
        int CreateTable(Table table);
        int UpdateTable(Table table);
        int DeleteTable(int id);
        bool BookTable(int id, string reservedUntil);
        bool ReleaseTable(int id);
    }
}
