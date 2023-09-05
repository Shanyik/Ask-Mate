using System.Data;
using Npgsql;

namespace AskMate.Model.Repositories;

public class UserRepository
{
    private readonly NpgsqlConnection _connection;
    
    public UserRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public List<User> GetAll()
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("SELECT * FROM users", _connection);

        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        var queryResult = new List<User>();
        foreach (DataRow row in table.Rows)
        {
            queryResult.Add(new User
            {
                Id = (int)row["id"],
                Username = (string)row["username"],
                Email = (string)row["email"],
                Password = (string)row["password"],
                RegistrationTime = (DateTime)row["registration_time"]
            });
        }
        _connection.Close();

        return queryResult;
    }
    
    public int Create(User user)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("INSERT INTO users(username, email, password, registration_time) VALUES (:username, :email, :password, :registration_time) RETURNING id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":username", user.Username);
        adapter.SelectCommand?.Parameters.AddWithValue(":email", user.Email);
        adapter.SelectCommand?.Parameters.AddWithValue(":password", user.Password);
        adapter.SelectCommand?.Parameters.AddWithValue(":registration_time", DateTime.Now);

        var lastInsertId = (int)adapter.SelectCommand?.ExecuteScalar();
        _connection.Close();

        return lastInsertId;
    }
}