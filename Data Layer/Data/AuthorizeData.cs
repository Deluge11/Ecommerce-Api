using Microsoft.Data.SqlClient;
using System.Data;
using Data_Layer.Interfaces;
using Microsoft.Extensions.Logging;
namespace Data_Layer.Data;

public class AuthorizeData : IAuthorizeData
{
    public string ConnectionString { get; }
    public ILogger<AuthorizeData> Logger { get; }

    public AuthorizeData (
        string connectionString,
        ILogger<AuthorizeData> logger
        )
    {
        ConnectionString = connectionString;
        Logger = logger;
    }


    public async Task<List<int>> GetPermissions(int userId)
    {
        List<int> permissions = new();

        using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
        using SqlCommand sqlCommand = new SqlCommand("GetPermissions", sqlConnection);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await sqlConnection.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(reader.GetInt32(reader.GetOrdinal("id")));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching permissions");
            return new List<int>();
        }
        return permissions;
    }
}
