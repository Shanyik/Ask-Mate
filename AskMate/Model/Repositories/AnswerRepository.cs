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

        var queryResult = new List<Answer>();
        foreach (DataRow row in table.Rows)
        {
            queryResult.Add(new Answer
            {
                Id = (int)row["id"],
                Message = (string)row["message"],
                QuestionId = (int)row["question_id"],
                SubmissionTime = (DateTime)row["submission_time"]
            });
        }
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
                SubmissionTime = (DateTime)row["submission_time"]
            });
        }
        _connection.Close();

        return queryResult;
    }
}