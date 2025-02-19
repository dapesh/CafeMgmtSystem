using CafeMgmtSystem.Services;
using Dapper;
using System.Data;
using Table = CafeMgmtSystem.Models.Table;

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
                var parameters = new DynamicParameters();
                parameters.Add("Flag", "s");
                parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output); 
                var tables = connection.Query<Table>(
                "sp_ManageTable",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                int result = parameters.Get<int>("Result");
                return tables;
            }
        }
        public Table GetTableById(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Flag", "g");
                parameters.Add("Id", id);
                parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var tables = connection.QuerySingleOrDefault<Table>(
                "sp_ManageTable",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                return tables;
            }
        }
        public int CreateTable(Table table)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Flag", "c");
                parameters.Add("Name", table.Name);
                parameters.Add("Seats", table.Seats);
                parameters.Add("IsAvailable", table.IsAvailable);
                parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var results = connection.Execute(
                "sp_ManageTable",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                int result = parameters.Get<int>("Result");
                return results;
            }
        }
        public  int UpdateTable(Table table)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Flag", "u");
                parameters.Add("Id", table.Id);
                parameters.Add("Name", table.Name);
                parameters.Add("Seats", table.Seats);
                parameters.Add("IsAvailable", table.IsAvailable);
                parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = connection.Execute(
                "sp_ManageTable",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        public int DeleteTable(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Flag", "d");
                parameters.Add("Id", id);
                parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var result = connection.Execute(
                "sp_ManageTable",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        public bool BookTable(int id, string reservedUntil)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Flag", "b");
                parameters.Add("ReservedUntil", reservedUntil);
                parameters.Add("Id", id);
                parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var affectedRows = connection.Execute(
                "sp_ManageTable",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                return affectedRows > 0;
            }
        }
        public bool ReleaseTable(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Flag", "r");
                parameters.Add("Id", id);
                parameters.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var affectedRows = connection.Execute(
                "sp_ManageTable",
                parameters,
                commandType: CommandType.StoredProcedure
                );
                return affectedRows > 0;
            }
        }
    }
}
