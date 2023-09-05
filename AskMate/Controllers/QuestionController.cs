using AskMate.Model;
using AskMate.Model.Repositories;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AskMate.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly string _connectionString =
        "Server=localhost;Port=5432;User Id=postgres;Password=asd;Database=askmate";

    [HttpGet]
    public IActionResult GetAll()
    {
        var repository = new QuestionRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.GetAll());
    }
    
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var repository = new QuestionRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.GetById(id));
    }

    [HttpPost()]
    public IActionResult Create(Question question)
    {
        var repository = new QuestionRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.Create(question));
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var repository = new QuestionRepository(new NpgsqlConnection(_connectionString));
        repository.Delete(id);

        return Ok();
    }
}