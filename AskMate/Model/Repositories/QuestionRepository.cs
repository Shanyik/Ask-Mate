﻿using System.Data;
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
        var adapter = new NpgsqlDataAdapter("SELECT * FROM questions" +
                                            "               ORDER BY submission_time", _connection);

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
}