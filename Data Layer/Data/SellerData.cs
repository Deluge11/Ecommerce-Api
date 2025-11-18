using Data_Layer.Interfaces;
using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.Data
{
    public class SellerData : ISellerData
    {

        public string ConnectionString { get; }
        public SellerData(string connectionString)
        {
            ConnectionString = connectionString;
        }


        public async Task<bool> ApplyForSeller(int userId)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            using SqlCommand sqlCommand = new SqlCommand("AddApplyForSellerRequest", sqlConnection);

            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

            try
            {
                await sqlConnection.OpenAsync();
                await sqlCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ConfirmSeller(int requestId)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            using SqlCommand sqlCommand = new SqlCommand("ConfirmSellerRequest", sqlConnection);

            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = requestId });

            try
            {
                await sqlConnection.OpenAsync();
                await sqlCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RefuseSeller(int requestId)
        {
            string query = "UPDATE ApplyForSellerRequests SET state = 0 WHERE id = @id AND state = NULL";

            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            sqlCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = requestId });

            try
            {
                await sqlConnection.OpenAsync();
                return await sqlCommand.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
