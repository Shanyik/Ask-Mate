using System.Data;
using Npgsql;

namespace AskMate.Model.Repositories;

public class QuestionRepository
{
    private readonly NpgsqlConnection _connection;
    
    public QuestionRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public List<Question> GetAll()
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("SELECT * FROM questions ORDER BY submission_time", _connection);

        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        var queryResult = (from DataRow row in table.Rows
        select new Question
        {
            Id = (int)row["id"],
            Title = (string)row["title"],
            Description = (string)row["description"],
            Author = (string)row["author"],
            SubmissionTime = (DateTime)row["submission_time"]
        }).ToList();
        _connection.Close();

        return queryResult;
    }

    public Question GetById(int id)
    {
        var answerRepository = new AnswerRepository(new NpgsqlConnection(_connection.ConnectionString));
        
        _connection.Open();
        
        var adapter = new NpgsqlDataAdapter("SELECT * FROM questions WHERE id = :id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":id", id);
        
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];
        
        if (table.Rows.Count > 0)
        {
            var row = table.Rows[0];
            var answers = answerRepository.GetAllByQuestionId((int)row["id"]);
            
            return new Question
            {
                Id = (int)row["id"],
                Title = (string)row["title"],
                Description = (string)row["description"],
                Author = (string)row["author"],
                SubmissionTime = (DateTime)row["submission_time"],
                Answers = answers
            };
        }
        
        _connection.Close();

        return null;
    }

    public int Create(string title, string description, string author)
    {

        Console.WriteLine(author);
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("INSERT INTO questions(title, description, submission_time, author) VALUES (:title, :description, :submission_time, :author) RETURNING id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":title", title);
        adapter.SelectCommand?.Parameters.AddWithValue(":description", description);
        adapter.SelectCommand?.Parameters.AddWithValue(":author", author);
        adapter.SelectCommand?.Parameters.AddWithValue(":submission_time", DateTime.Now);

        var lastInsertId = (int)adapter.SelectCommand?.ExecuteScalar();
        _connection.Close();

        return lastInsertId;
    }
    
    public void Delete(int id)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter(
            "DELETE FROM questions WHERE id = :id",
            _connection
        );
        adapter.SelectCommand?.Parameters.AddWithValue(":id", id);

        adapter.SelectCommand?.ExecuteNonQuery();
        _connection.Close();
    }
}