using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Dapper;
using System.Data;

namespace CafeMgmtSystem.Repository
{
    public class TableRepository : ITableRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public TableRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        private IDbConnection Connection => _dbConnectionFactory.CreateConnection();

        public IEnumerable<Table> GetAllTables()
        {
            using (var connection = Connection)
            {
                connection.Open();
                var tables =  connection.Query<Table>("GetAllTables", commandType: CommandType.StoredProcedure);
                return tables;
            }
        }

        public Table GetTableById(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var table =  connection.QuerySingleOrDefault<Table>(
                    "GetTableById", 
                    new { Id = id }, 
                    commandType: CommandType.StoredProcedure
                );
                return table;
            }
        }

        public int CreateTable(Table table)
        {
            using (var connection = Connection)
            {
                 connection.Open();
                var result =  connection.Execute(
                    "CreateTable", 
                    new 
                    { 
                        table.Name, 
                        table.Seats, 
                        table.IsAvailable 
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result; 
            }
        }
        public  int UpdateTable(Table table)
        {
            using (var connection = Connection)
            {
                 connection.Open();
                var result =  connection.Execute(
                    "UpdateTable", 
                    new 
                    { 
                        table.Id, 
                        table.Name, 
                        table.Seats, 
                        table.IsAvailable 
                    },
                    commandType: CommandType.StoredProcedure
                );
                return result; 
            }
        }

        public  int DeleteTable(int id)
        {
            using (var connection = Connection)
            {
                 connection.Open();
                var result =  connection.Execute(
                    "DeleteTable", 
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );
                return result; 
            }
        }

        public bool BookTable(int id, DateTime reservedUntil)
        {
            using (var connection = Connection)
            {
                var affectedRows = connection.Execute(
                    "BookTable",
                    new { Id = id, ReservedUntil = reservedUntil },
                    commandType: CommandType.StoredProcedure
                );
                return affectedRows > 0;
            }
        }

        public bool ReleaseTable(int id)
        {
            using (var connection = Connection)
            {
                var affectedRows = connection.Execute(
                    "ReleaseTable",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );
                return affectedRows > 0;
            }
        }
    }
}
