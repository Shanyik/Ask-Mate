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

        var queryResult = new List<Question>();
        foreach (DataRow row in table.Rows)
        {
            queryResult.Add(new Question
            {
                Id = (int)row["id"],
                Title = (string)row["title"],
                Description = (string)row["description"],
                SubmissionTime = (DateTime)row["submission_time"]
            });
        }
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
            DataRow row = table.Rows[0];
            List<Answer> answers = answerRepository.GetAllByQuestionId((int)row["id"]);
            
            return new Question
            {
                Id = (int)row["id"],
                Title = (string)row["title"],
                Description = (string)row["description"],
                SubmissionTime = (DateTime)row["submission_time"],
                Answers = answers
            };
        }
        
        _connection.Close();

        return null;
    }

    public int Create(Question question)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("INSERT INTO questions(title, description, submission_time) VALUES (:title, :author, :submission_time) RETURNING id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":title", question.Title);
        adapter.SelectCommand?.Parameters.AddWithValue(":author", question.Title);
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