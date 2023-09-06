using System.Data;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        var queryResult = (from DataRow row in table.Rows
        select new User
        {
            Id = (int)row["id"],
            Username = (string)row["username"],
            Email = (string)row["email"],
            Password = (string)row["password"],
            RegistrationTime = (DateTime)row["registration_time"]
        }).ToList();
        _connection.Close();

        return queryResult;
    }
    
    public int Create(string username, string email, string password)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("INSERT INTO users(username, email, password, registration_time) VALUES (:username, :email, :password, :registration_time) RETURNING id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":username", username);
        adapter.SelectCommand?.Parameters.AddWithValue(":email", email);
        adapter.SelectCommand?.Parameters.AddWithValue(":password", password);
        adapter.SelectCommand?.Parameters.AddWithValue(":registration_time", DateTime.Now);

        var lastInsertId = (int)adapter.SelectCommand?.ExecuteScalar();
        _connection.Close();

        return lastInsertId;
    }

    public List<Claim> Login(string username, string email, string password)
    {
        _connection.Open();

        var adapter = new NpgsqlDataAdapter("SELECT * FROM users WHERE username = :username", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":username", username);

        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        if (table.Rows.Count > 0)
        {
            var row = table.Rows[0];

            if ((username == (string)row["username"] || email == (string)row["email"]) &&
                password == (string)row["password"])
            {
                _connection.Close();
                return new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "user")
                };
            }
        }
        _connection.Close();
        return null;
    }
}