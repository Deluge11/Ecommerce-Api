
using Microsoft.Data.SqlClient;
using System.Data;
using Data_Layer.Interfaces;
using Models;


namespace Data_Layer.Data;

public class UsersData : IUsersData
{
    private string ConnectionString { get; }

    public UsersData(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        string query = "SELECT id, name ,password FROM Users WHERE email = @email";
        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar) { Value = email });

        try
        {
            await sqlConnect.OpenAsync();
            using var reader = await sqlcommand.ExecuteReaderAsync();
            await reader.ReadAsync();

            return new User
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                password = reader.GetString(reader.GetOrdinal("password"))
            };
        }
        catch (Exception)
        {
            throw;
            return null;
        }
    }
    public async Task<User> InsertUser(string name, string email, string password)
    {
        string query = @"INSERT INTO Users (name,email,password) Values (@name,@email,@password);
                             SELECT CAST(scope_identity() AS int)";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionString);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = name });
        sqlcommand.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar) { Value = email });
        sqlcommand.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });

        try
        {
            await sqlConnect.OpenAsync();
            var objId = (int)await sqlcommand.ExecuteScalarAsync();
            return new User
            {
                id = objId,
                name = name
            };
        }
        catch (Exception)
        {
            return null;
        }

    }

}