using System.Data;
using Npgsql;

namespace AskMate.Model.Repositories;

public class AnswerRepository
{
    private readonly NpgsqlConnection _connection;
    
    public AnswerRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public List<Answer> GetAll()
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("SELECT * FROM answers ORDER BY submission_time", _connection);

        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        var queryResult = (from DataRow row in table.Rows
            select new Answer
            {
                Id = (int)row["id"],
                Message = (string)row["message"],
                QuestionId = (int)row["question_id"],
                Accepted = (bool)row["accepted"],
                SubmissionTime = (DateTime)row["submission_time"]
            }).ToList();
        _connection.Close();

        return queryResult;
    }
    
    public List<Answer> GetAllByQuestionId(int id)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("SELECT * FROM answers WHERE question_id = :id ORDER BY submission_time", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":id", id);

        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        var queryResult = new List<Answer>();
        foreach (DataRow row in table.Rows)
        {
            queryResult.Add(new Answer
            {
                Id = (int)row["id"],
                Message = (string)row["message"],
                QuestionId = (int)row["question_id"],
                Accepted = (bool)row["accepted"],
                SubmissionTime = (DateTime)row["submission_time"]
            });
        }
        _connection.Close();

        return queryResult;
    }
    
    public int Create(string message, int questionId)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("INSERT INTO answers(message, question_id, submission_time) VALUES (:message, :question_id, :submission_time) RETURNING id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":message", message);
        adapter.SelectCommand?.Parameters.AddWithValue(":question_id", questionId);
        adapter.SelectCommand?.Parameters.AddWithValue(":submission_time", DateTime.Now);

        var lastInsertId = (int)adapter.SelectCommand?.ExecuteScalar();
        _connection.Close();

        return lastInsertId;
    }
    
    public void Delete(int id)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter(
            "DELETE FROM answers WHERE id = :id",
            _connection
        );
        adapter.SelectCommand?.Parameters.AddWithValue(":id", id);

        adapter.SelectCommand?.ExecuteNonQuery();
        _connection.Close();
    }

    public void Accept(int id, String author)
    {
        _connection.Open();
        
        var adapter = new NpgsqlDataAdapter("SELECT * FROM questions JOIN answers on questions.id = answers.question_id WHERE answers.id = :id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":id", id);
        
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        if (table.Rows.Count > 0)
        {
            DataRow row = table.Rows[0];
            if ((string)row["author"] == author)
            {
                adapter = new NpgsqlDataAdapter(
                    "UPDATE answers SET accepted = :accepted WHERE id = :id",
                    _connection
                ); 
        
                adapter.SelectCommand?.Parameters.AddWithValue(":id", id);
                adapter.SelectCommand?.Parameters.AddWithValue(":accepted", true);
        
                adapter.SelectCommand?.ExecuteNonQuery();
            }
        }
        
        _connection.Close();
    }
}