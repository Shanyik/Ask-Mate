using AskMate.Model;
using AskMate.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AskMate.Controllers;

[ApiController]
[Route("[controller]")]
public class AnswerController : ControllerBase
{
    private readonly string _connectionString =
        "Server=localhost;Port=5432;User Id=postgres;Password=asd;Database=askmate";

    [HttpGet, ]
    public IActionResult GetAll()
    {
        var repository = new AnswerRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.GetAll());
    }
    
    [HttpPost(), Authorize]
    public IActionResult Create(Answer answer)
    {
        var repository = new AnswerRepository(new NpgsqlConnection(_connectionString));

        return Ok(repository.Create(answer));
    }
    
    [HttpDelete("{id}"), Authorize]
    public IActionResult Delete(int id)
    {
        var repository = new AnswerRepository(new NpgsqlConnection(_connectionString));
        repository.Delete(id);

        return Ok();
    }
}