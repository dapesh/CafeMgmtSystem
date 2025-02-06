
using CafeMgmtSystem.Models;
using CafeMgmtSystem.Services;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CafeMgmtSystem.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public PaymentRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        private IDbConnection Connection => _dbConnectionFactory.CreateConnection();

        public async Task<bool> UpdateOrderStatusAsync(int orderId, LatestOrderStatus status)
        {
            using (var connection = Connection)
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@OrderId", orderId, DbType.Int32);
                parameters.Add("@Status", status.ToString(), DbType.String);
                //var result = await connection.ExecuteAsync("sp_UpdateOrderStatus", parameters, commandType: CommandType.StoredProcedure);
                //int rowsAffected = await connection.ExecuteAsync("sp_UpdateOrderStatus", parameters, commandType: CommandType.StoredProcedure);
                var result = await connection.QuerySingleOrDefaultAsync<int>(
                    "sp_UpdateOrderStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result == 1;
                //return rowsAffected > 0;
            }
        }
    }
}
